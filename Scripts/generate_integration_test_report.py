#!/usr/bin/env python3
"""
Integration Test Report Generator for .NET CMA SDK
Parses TRX (results) + Cobertura (coverage) + Structured StdOut (HTTP, assertions, context)
into a single interactive HTML report.

SECURITY FEATURES:
- Uses defusedxml for secure XML parsing to prevent XXE attacks
- Robust path traversal prevention for all file operations  
- Input validation and sanitization for all user-provided paths
- No insecure XML parsing fallbacks (security-first approach)

Requirements:
- defusedxml>=0.7.1 (REQUIRED for secure XML parsing)
- Python 3.7+ recommended
"""

import xml.etree.ElementTree as ET
import os
import sys
import re
import json
import argparse
from datetime import datetime

# Import defusedxml for secure XML parsing (required dependency)
try:
    import defusedxml.ElementTree as SafeET
    DEFUSED_XML_AVAILABLE = True
except ImportError as e:
    print("ERROR: defusedxml is required for secure XML parsing.")
    print("Install it with: pip install defusedxml")
    print("This prevents XXE vulnerabilities in XML processing.")
    sys.exit(1)




def _sanitize_output_path(output_path):
    """
    Robust path traversal prevention: output must resolve under the current working directory.
    Prevents directory traversal attacks and validates file path safety.
    """
    if not output_path or not isinstance(output_path, str):
        raise ValueError("Invalid output path: path must be a non-empty string")
    
    # Check for null bytes and other dangerous characters
    if '\x00' in output_path:
        raise ValueError("Invalid output path: contains null byte")
    
    # Check for dangerous path components
    dangerous_patterns = ['..', '~/', '\\..\\', '/../', '\\.\\', '/./']
    for pattern in dangerous_patterns:
        if pattern in output_path:
            raise ValueError(f"Invalid output path: contains dangerous pattern '{pattern}'")
    
    # Resolve paths safely
    cwd = os.path.abspath(os.getcwd())
    try:
        candidate = os.path.abspath(os.path.normpath(output_path))
    except (OSError, ValueError) as e:
        raise ValueError(f"Invalid output path: cannot resolve path: {e}") from e
    
    # Ensure the resolved path is under the working directory
    try:
        # Use os.path.commonpath for cross-platform safety
        common = os.path.commonpath([cwd, candidate])
    except ValueError as e:
        raise ValueError(
            "Output path must be on the same drive as the working directory "
            "and must not escape it (path traversal)."
        ) from e
    
    if not common.startswith(cwd) or common != cwd:
        raise ValueError(
            f"Output path must be inside the working directory ({cwd}). Refusing: {output_path!r}"
        )
    
    # Additional check: ensure no symlink attacks
    if os.path.islink(os.path.dirname(candidate)) and os.path.dirname(candidate) != cwd:
        raise ValueError("Output path directory cannot be a symbolic link outside working directory")
    
    return candidate


class IntegrationTestReportGenerator:
    def __init__(self, trx_path, coverage_path=None):
        self.trx_path = trx_path
        self.coverage_path = coverage_path
        self.results = {
            'total': 0,
            'passed': 0,
            'failed': 0,
            'skipped': 0,
            'duration_seconds': 0,
            'tests': []
        }
        self.coverage = {
            'lines_pct': 0,
            'branches_pct': 0,
            'statements_pct': 0,
            'functions_pct': 0
        }
        self.file_coverage = []

    # ──────────────────── TRX PARSING ────────────────────

    def parse_trx(self):
        # Safely parse TRX file with defusedxml (required for security)
        try:
            tree = SafeET.parse(self.trx_path)
            root = tree.getroot()
        except Exception as e:
            raise ValueError(f"Failed to parse TRX file safely: {e}") from e
        ns = {'t': 'http://microsoft.com/schemas/VisualStudio/TeamTest/2010'}

        unit_tests_by_id = {}
        for ut in root.findall('.//t:UnitTest', ns):
            tid = ut.get('id')
            if tid:
                unit_tests_by_id[tid] = ut

        counters = root.find('.//t:ResultSummary/t:Counters', ns)
        if counters is not None:
            self.results['total'] = int(counters.get('total', 0))
            self.results['passed'] = int(counters.get('passed', 0))
            self.results['failed'] = int(counters.get('failed', 0))
            self.results['skipped'] = int(counters.get('notExecuted', 0))

        times = root.find('.//t:Times', ns)
        if times is not None:
            try:
                start = times.get('start', '')
                finish = times.get('finish', '')
                if start and finish:
                    fmt = '%Y-%m-%dT%H:%M:%S.%f'
                    s = start.split('+')[0].split('-')[0:3]
                    start_clean = re.sub(r'[+-]\d{2}:\d{2}$', '', start)
                    finish_clean = re.sub(r'[+-]\d{2}:\d{2}$', '', finish)
                    for fmt_try in ['%Y-%m-%dT%H:%M:%S.%f', '%Y-%m-%dT%H:%M:%S']:
                        try:
                            dt_start = datetime.strptime(start_clean, fmt_try)
                            dt_finish = datetime.strptime(finish_clean, fmt_try)
                            self.results['duration_seconds'] = (dt_finish - dt_start).total_seconds()
                            break
                        except ValueError:
                            continue
            except Exception:
                pass

        integration_total = 0
        integration_passed = 0
        integration_failed = 0
        integration_skipped = 0

        for result in root.findall('.//t:UnitTestResult', ns):
            test_id = result.get('testId', '')
            test_name = result.get('testName', '')
            outcome = result.get('outcome', 'Unknown')
            duration_str = result.get('duration', '0')
            duration = self._parse_duration(duration_str)

            ut_el = unit_tests_by_id.get(test_id)
            test_def = ut_el.find('t:TestMethod', ns) if ut_el is not None else None
            class_name = test_def.get('className', '') if test_def is not None else ''

            if 'IntegrationTest' not in class_name:
                continue

            parts = class_name.split(',')[0].rsplit('.', 1)
            file_name = parts[-1] if len(parts) > 1 else class_name

            error_msg = error_trace = None
            error_info = result.find('.//t:ErrorInfo', ns)
            if error_info is not None:
                msg_el = error_info.find('t:Message', ns)
                stk_el = error_info.find('t:StackTrace', ns)
                if msg_el is not None:
                    error_msg = msg_el.text
                if stk_el is not None:
                    error_trace = stk_el.text

            structured = None
            stdout_el = result.find('.//t:StdOut', ns)
            if stdout_el is not None and stdout_el.text:
                structured = self._parse_structured_output(stdout_el.text)

            integration_total += 1
            if outcome == 'Passed':
                integration_passed += 1
            elif outcome == 'Failed':
                integration_failed += 1
            elif outcome in ('NotExecuted', 'Inconclusive'):
                integration_skipped += 1

            self.results['tests'].append({
                'name': test_name,
                'outcome': outcome,
                'duration': duration,
                'file': file_name,
                'error_message': error_msg,
                'error_stacktrace': error_trace,
                'structured': structured
            })

        self.results['total'] = integration_total
        self.results['passed'] = integration_passed
        self.results['failed'] = integration_failed
        self.results['skipped'] = integration_skipped

    def _parse_duration(self, duration_str):
        try:
            parts = duration_str.split(':')
            if len(parts) == 3:
                h, m = int(parts[0]), int(parts[1])
                s = float(parts[2])
                total = h * 3600 + m * 60 + s
                return f"{total:.2f}s"
        except Exception:
            pass
        return duration_str

    # ──────────────────── COBERTURA PARSING ────────────────────

    def parse_coverage(self):
        if not self.coverage_path or not os.path.exists(self.coverage_path):
            return
        try:
            # Safely parse coverage file with defusedxml (required for security)
            tree = SafeET.parse(self.coverage_path)
            root = tree.getroot()
            self.coverage['lines_pct'] = float(root.get('line-rate', 0)) * 100
            self.coverage['branches_pct'] = float(root.get('branch-rate', 0)) * 100
            self.coverage['statements_pct'] = self.coverage['lines_pct']

            total_methods = 0
            covered_methods = 0
            for method in root.iter('method'):
                total_methods += 1
                lr = float(method.get('line-rate', 0))
                if lr > 0:
                    covered_methods += 1
            if total_methods > 0:
                self.coverage['functions_pct'] = (covered_methods / total_methods) * 100

            self._parse_file_coverage(root)
        except Exception as e:
            print(f"Warning: Could not parse coverage file: {e}")

    def _parse_file_coverage(self, root):
        file_data = {}
        for cls in root.iter('class'):
            filename = cls.get('filename', '')
            if not filename:
                continue

            if filename not in file_data:
                file_data[filename] = {
                    'lines': {},
                    'branches_covered': 0,
                    'branches_total': 0,
                    'methods_total': 0,
                    'methods_covered': 0,
                }

            entry = file_data[filename]

            for method in cls.findall('methods/method'):
                entry['methods_total'] += 1
                if float(method.get('line-rate', 0)) > 0:
                    entry['methods_covered'] += 1

            for line in cls.iter('line'):
                num = int(line.get('number', 0))
                hits = int(line.get('hits', 0))
                is_branch = line.get('branch', 'False').lower() == 'true'

                if num in entry['lines']:
                    entry['lines'][num]['hits'] = max(entry['lines'][num]['hits'], hits)
                    if is_branch:
                        entry['lines'][num]['is_branch'] = True
                        cond = line.get('condition-coverage', '')
                        covered, total = self._parse_condition_coverage(cond)
                        entry['lines'][num]['br_covered'] = max(entry['lines'][num].get('br_covered', 0), covered)
                        entry['lines'][num]['br_total'] = max(entry['lines'][num].get('br_total', 0), total)
                else:
                    br_covered, br_total = 0, 0
                    if is_branch:
                        cond = line.get('condition-coverage', '')
                        br_covered, br_total = self._parse_condition_coverage(cond)
                    entry['lines'][num] = {
                        'hits': hits,
                        'is_branch': is_branch,
                        'br_covered': br_covered,
                        'br_total': br_total,
                    }

        self.file_coverage = []
        for filename in sorted(file_data.keys()):
            entry = file_data[filename]
            lines_total = len(entry['lines'])
            lines_covered = sum(1 for l in entry['lines'].values() if l['hits'] > 0)
            uncovered = sorted(num for num, l in entry['lines'].items() if l['hits'] == 0)

            br_total = sum(l.get('br_total', 0) for l in entry['lines'].values() if l.get('is_branch'))
            br_covered = sum(l.get('br_covered', 0) for l in entry['lines'].values() if l.get('is_branch'))

            self.file_coverage.append({
                'filename': filename,
                'lines_pct': (lines_covered / lines_total * 100) if lines_total > 0 else 100,
                'statements_pct': (lines_covered / lines_total * 100) if lines_total > 0 else 100,
                'branches_pct': (br_covered / br_total * 100) if br_total > 0 else 100,
                'functions_pct': (entry['methods_covered'] / entry['methods_total'] * 100) if entry['methods_total'] > 0 else 100,
                'uncovered_lines': uncovered,
            })

    @staticmethod
    def _parse_condition_coverage(cond_str):
        m = re.match(r'(\d+)%\s*\((\d+)/(\d+)\)', cond_str)
        if m:
            return int(m.group(2)), int(m.group(3))
        return 0, 0

    @staticmethod
    def _collapse_line_ranges(lines):
        if not lines:
            return ''
        ranges = []
        start = prev = lines[0]
        for num in lines[1:]:
            if num == prev + 1:
                prev = num
            else:
                ranges.append(f"{start}-{prev}" if start != prev else str(start))
                start = prev = num
        ranges.append(f"{start}-{prev}" if start != prev else str(start))
        return ','.join(ranges)

    # ──────────────────── STRUCTURED OUTPUT ────────────────────

    def _parse_structured_output(self, text):
        data = {
            'assertions': [],
            'requests': [],
            'responses': [],
            'context': []
        }
        pattern = r'###TEST_OUTPUT_START###(.+?)###TEST_OUTPUT_END###'
        for match in re.findall(pattern, text, re.DOTALL):
            try:
                obj = json.loads(match)
                t = obj.get('type', '').upper()
                if t == 'ASSERTION':
                    data['assertions'].append({
                        'name': obj.get('assertionName', ''),
                        'expected': obj.get('expected', ''),
                        'actual': obj.get('actual', ''),
                        'passed': obj.get('passed', True)
                    })
                elif t == 'HTTP_REQUEST':
                    data['requests'].append({
                        'method': obj.get('method', ''),
                        'url': obj.get('url', ''),
                        'headers': obj.get('headers', {}),
                        'body': obj.get('body', ''),
                        'curl': obj.get('curlCommand', ''),
                        'sdkMethod': obj.get('sdkMethod', '')
                    })
                elif t == 'HTTP_RESPONSE':
                    data['responses'].append({
                        'statusCode': obj.get('statusCode', 0),
                        'statusText': obj.get('statusText', ''),
                        'headers': obj.get('headers', {}),
                        'body': obj.get('body', '')
                    })
                elif t == 'CONTEXT':
                    data['context'].append({
                        'key': obj.get('key', ''),
                        'value': obj.get('value', '')
                    })
            except json.JSONDecodeError:
                continue
        return data

    # ──────────────────── HTML HELPERS ────────────────────

    @staticmethod
    def _esc(text):
        if text is None:
            return ""
        text = str(text)
        return (text
                .replace('&', '&amp;')
                .replace('<', '&lt;')
                .replace('>', '&gt;')
                .replace('"', '&quot;')
                .replace("'", '&#39;'))

    def _format_duration_display(self, seconds):
        if seconds < 60:
            return f"{seconds:.1f}s"
        elif seconds < 3600:
            m = int(seconds // 60)
            s = seconds % 60
            return f"{m}m {s:.0f}s"
        else:
            h = int(seconds // 3600)
            m = int((seconds % 3600) // 60)
            return f"{h}h {m}m"

    # ──────────────────── HTML GENERATION ────────────────────

    def generate_html(self, output_path):
        output_path = _sanitize_output_path(output_path)
        pass_rate = (self.results['passed'] / self.results['total'] * 100) if self.results['total'] > 0 else 0
        duration_display = self._format_duration_display(self.results['duration_seconds'])

        by_file = {}
        for test in self.results['tests']:
            by_file.setdefault(test['file'], []).append(test)

        html = self._html_head()
        html += self._html_header(pass_rate)
        html += self._html_kpi_bar(duration_display)
        html += self._html_pass_rate(pass_rate)
        html += self._html_coverage_table()
        html += self._html_test_navigation(by_file)
        html += self._html_file_coverage_table()
        html += self._html_footer()
        html += self._html_scripts()
        html += "</div></body></html>"

        with open(output_path, 'w', encoding='utf-8') as f:
            f.write(html)
        return os.path.abspath(output_path)

    def _html_head(self):
        return f"""<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>.NET CMA SDK - Integration Test Report</title>
    <style>
        * {{ margin: 0; padding: 0; box-sizing: border-box; }}
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 20px;
            color: #333;
        }}
        .container {{
            max-width: 1600px;
            margin: 0 auto;
            background: white;
            border-radius: 12px;
            box-shadow: 0 20px 60px rgba(0,0,0,0.3);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 40px;
            text-align: center;
        }}
        .header h1 {{ font-size: 2.5em; margin-bottom: 10px; font-weight: 700; }}
        .header p {{ font-size: 1.1em; opacity: 0.9; }}
        .summary {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 20px;
            padding: 40px;
            background: #f8f9fa;
        }}
        .summary-card {{
            background: white;
            padding: 25px;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            text-align: center;
            transition: transform 0.2s;
        }}
        .summary-card:hover {{ transform: translateY(-5px); box-shadow: 0 4px 12px rgba(0,0,0,0.15); }}
        .summary-card .number {{ font-size: 3em; font-weight: bold; margin-bottom: 10px; }}
        .summary-card .label {{ font-size: 0.9em; color: #666; text-transform: uppercase; letter-spacing: 1px; }}
        .clr-passed {{ color: #28a745; }}
        .clr-failed {{ color: #dc3545; }}
        .clr-skipped {{ color: #ffc107; }}
        .clr-total {{ color: #007bff; }}
        .clr-duration {{ color: #6f42c1; }}
        .pass-rate {{ padding: 30px 40px; background: white; text-align: center; border-top: 3px solid #f8f9fa; }}
        .pass-rate-bar {{ width: 100%; height: 40px; background: #e9ecef; border-radius: 20px; overflow: hidden; margin: 20px 0; }}
        .pass-rate-fill {{
            height: 100%;
            background: linear-gradient(90deg, #28a745 0%, #20c997 100%);
            transition: width 1s ease-out;
            display: flex; align-items: center; justify-content: center;
            color: white; font-weight: bold; font-size: 1.1em;
        }}
        .coverage-section {{ padding: 30px 40px; background: #f8f9fa; }}
        .coverage-section h2 {{ margin-bottom: 20px; font-size: 1.5em; }}
        .coverage-table {{ width: 100%; border-collapse: collapse; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .coverage-table th {{ background: #667eea; color: white; padding: 15px; text-align: center; font-weight: 600; text-transform: uppercase; font-size: 0.85em; letter-spacing: 0.5px; }}
        .coverage-table td {{ padding: 15px; text-align: center; font-size: 1.3em; font-weight: 700; }}
        .cov-good {{ color: #28a745; }}
        .cov-warn {{ color: #ffc107; }}
        .cov-bad {{ color: #dc3545; }}
        .file-coverage-section {{ margin-top: 0; border-top: 3px solid #e9ecef; }}
        .file-cov-table td {{ font-size: 0.95em; font-weight: 600; padding: 10px 15px; border-bottom: 1px solid #e9ecef; }}
        .file-cov-table tr:last-child td {{ border-bottom: none; }}
        .file-cov-table tbody tr:hover {{ background: #f8f9fa; }}
        .fc-summary-row {{ background: #f0f2ff; }}
        .fc-summary-row td {{ border-bottom: 2px solid #667eea !important; }}
        .fc-file-col {{ text-align: left !important; }}
        .fc-file-cell {{ text-align: left !important; font-family: 'Consolas', 'Monaco', monospace; font-size: 0.88em !important; }}
        .fc-dir {{ color: #888; }}
        .fc-uncov-col {{ text-align: left !important; }}
        .fc-uncov-cell {{ text-align: left !important; font-family: 'Consolas', 'Monaco', monospace; font-size: 0.82em !important; color: #dc3545; font-weight: 400 !important; }}
        .test-results {{ padding: 40px; }}
        .test-results > h2 {{ margin-bottom: 30px; font-size: 2em; }}
        .category {{ margin-bottom: 30px; }}
        .category-header {{
            background: #f8f9fa; padding: 15px 20px; border-left: 4px solid #667eea;
            margin-bottom: 15px; border-radius: 4px; cursor: pointer;
            display: flex; justify-content: space-between; align-items: center; transition: background 0.2s;
        }}
        .category-header:hover {{ background: #e9ecef; }}
        .category-title {{ font-size: 1.3em; font-weight: 600; color: #333; }}
        .category-stats {{ font-size: 0.9em; color: #666; }}
        .test-table {{ width: 100%; border-collapse: collapse; margin-bottom: 20px; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .test-table thead {{ background: #667eea; color: white; }}
        .test-table th {{ padding: 15px; text-align: left; font-weight: 600; text-transform: uppercase; font-size: 0.85em; letter-spacing: 0.5px; }}
        .test-table td {{ padding: 15px; border-bottom: 1px solid #e9ecef; }}
        .test-table tr:last-child td {{ border-bottom: none; }}
        .test-table tbody tr:hover {{ background: #f8f9fa; }}
        .test-name {{ font-family: 'Consolas', 'Monaco', monospace; font-size: 0.9em; cursor: pointer; color: #007bff; }}
        .test-name:hover {{ text-decoration: underline; }}
        .status-badge {{ display: inline-block; padding: 6px 12px; border-radius: 20px; font-size: 0.85em; font-weight: 600; text-transform: uppercase; }}
        .status-passed {{ background: #d4edda; color: #155724; }}
        .status-failed {{ background: #f8d7da; color: #721c24; }}
        .status-skipped {{ background: #fff3cd; color: #856404; }}
        .test-details-container {{ background: #f8f9fa; border-left: 4px solid #007bff; margin-top: 15px; padding: 20px; border-radius: 4px; display: none; }}
        .test-details-container.show {{ display: block; }}
        .test-details-section {{ margin-bottom: 25px; }}
        .details-heading {{ font-size: 1.1em; color: #333; margin-bottom: 15px; padding-bottom: 10px; border-bottom: 2px solid #e9ecef; }}
        .assertions-table {{ background: white; border-radius: 6px; overflow: hidden; }}
        .assertion-row {{ padding: 15px; border-bottom: 1px solid #e9ecef; }}
        .assertion-row:last-child {{ border-bottom: none; }}
        .assertion-row.a-failed {{ background: #fff5f5; }}
        .assertion-header {{ display: flex; align-items: center; gap: 10px; margin-bottom: 10px; }}
        .assertion-icon {{ font-size: 1.2em; }}
        .expected-actual-row {{ display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-top: 10px; }}
        .ea-label {{ font-weight: 600; color: #666; font-size: 0.85em; text-transform: uppercase; display: block; margin-bottom: 5px; }}
        .value-box {{
            background: #f8f9fa; padding: 10px; border-radius: 4px; border: 1px solid #dee2e6;
            font-family: 'Consolas', 'Monaco', monospace; font-size: 0.85em;
            max-height: 200px; overflow-y: auto; white-space: pre-wrap; word-break: break-all;
        }}
        .request-block, .response-block {{ background: white; padding: 15px; border-radius: 6px; margin-bottom: 15px; border: 1px solid #dee2e6; }}
        .sdk-method-badge {{
            background: linear-gradient(135deg, #e8f5e9, #f1f8e9); border: 1px solid #81c784;
            border-radius: 6px; padding: 6px 12px; margin-bottom: 8px; font-size: 0.88em; color: #2e7d32;
        }}
        .sdk-method-badge code {{ background: #c8e6c9; padding: 2px 8px; border-radius: 4px; font-family: 'Consolas', 'Monaco', monospace; font-size: 0.95em; font-weight: 600; color: #1b5e20; }}
        .request-summary, .response-summary {{ display: flex; align-items: center; gap: 10px; margin-bottom: 10px; }}
        .http-method {{ background: #007bff; color: white; padding: 4px 10px; border-radius: 4px; font-weight: 600; font-size: 0.85em; }}
        .http-url {{ font-family: 'Consolas', 'Monaco', monospace; font-size: 0.9em; color: #333; word-break: break-all; }}
        .http-status {{ font-weight: 600; font-size: 1.1em; }}
        .response-summary.rs-success .http-status {{ color: #28a745; }}
        .response-summary.rs-error .http-status {{ color: #dc3545; }}
        details.req-details, details.res-details {{ margin-top: 10px; }}
        details.req-details summary, details.res-details summary {{ cursor: pointer; padding: 8px 12px; background: #f8f9fa; border-radius: 4px; font-weight: 600; font-size: 0.9em; user-select: none; }}
        details.req-details summary:hover, details.res-details summary:hover {{ background: #e9ecef; }}
        .headers-box, .body-box, .curl-box {{
            background: #f8f9fa; padding: 12px; border-radius: 4px; border: 1px solid #dee2e6;
            font-family: 'Consolas', 'Monaco', monospace; font-size: 0.85em;
            margin-top: 10px; max-height: 300px; overflow-y: auto; white-space: pre-wrap; word-break: break-all;
        }}
        .curl-section {{ background: #f0f7ff; border-left: 3px solid #007bff; }}
        .copy-btn {{ background: #007bff; color: white; border: none; padding: 6px 12px; border-radius: 4px; cursor: pointer; font-size: 0.85em; margin-top: 5px; transition: background 0.2s; }}
        .copy-btn:hover {{ background: #0056b3; }}
        .show-more-btn {{ background: #6c757d; color: white; border: none; padding: 4px 10px; border-radius: 4px; cursor: pointer; font-size: 0.8em; margin-top: 5px; }}
        .error-details {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin-top: 10px; border-radius: 4px; }}
        .error-message {{ font-family: 'Consolas', 'Monaco', monospace; font-size: 0.9em; color: #721c24; white-space: pre-wrap; margin-bottom: 10px; }}
        .error-stacktrace {{ font-family: 'Consolas', 'Monaco', monospace; font-size: 0.8em; color: #666; white-space: pre-wrap; background: #f8f9fa; padding: 10px; border-radius: 4px; max-height: 300px; overflow-y: auto; }}
        .collapsible {{ display: none; }}
        .collapsible.active {{ display: block; }}
        .toggle-icon {{ transition: transform 0.3s; }}
        .toggle-icon.rotated {{ transform: rotate(90deg); }}
        .footer {{ background: #f8f9fa; padding: 30px; text-align: center; color: #666; border-top: 3px solid #e9ecef; }}
        .footer p {{ margin: 5px 0; }}
        @media print {{
            body {{ background: white; padding: 0; }}
            .container {{ box-shadow: none; }}
            .collapsible {{ display: block !important; }}
            .test-details-container {{ display: block !important; }}
        }}
    </style>
</head>
<body>
<div class="container">
"""

    def _html_header(self, pass_rate):
        now = datetime.now().strftime('%B %d, %Y at %I:%M %p')
        return f"""
    <div class="header">
        <h1>Integration Test Results</h1>
        <p>.NET CMA SDK &mdash; {now}</p>
    </div>
"""

    def _html_kpi_bar(self, duration_display):
        r = self.results
        return f"""
    <div class="summary">
        <div class="summary-card"><div class="number clr-total">{r['total']}</div><div class="label">Total Tests</div></div>
        <div class="summary-card"><div class="number clr-passed">{r['passed']}</div><div class="label">Passed</div></div>
        <div class="summary-card"><div class="number clr-failed">{r['failed']}</div><div class="label">Failed</div></div>
        <div class="summary-card"><div class="number clr-skipped">{r['skipped']}</div><div class="label">Skipped</div></div>
        <div class="summary-card"><div class="number clr-duration">{duration_display}</div><div class="label">Duration</div></div>
    </div>
"""

    def _html_pass_rate(self, pass_rate):
        return f"""
    <div class="pass-rate">
        <h2>Pass Rate</h2>
        <div class="pass-rate-bar">
            <div class="pass-rate-fill" style="width: {pass_rate}%">{pass_rate:.1f}%</div>
        </div>
    </div>
"""

    def _html_coverage_table(self):
        c = self.coverage
        if c['lines_pct'] == 0 and c['branches_pct'] == 0:
            return ""

        def cov_class(pct):
            if pct >= 80: return 'cov-good'
            if pct >= 50: return 'cov-warn'
            return 'cov-bad'

        return f"""
    <div class="coverage-section">
        <h2>Global Code Coverage</h2>
        <table class="coverage-table">
            <thead><tr>
                <th>Statements</th><th>Branches</th><th>Functions</th><th>Lines</th>
            </tr></thead>
            <tbody><tr>
                <td class="{cov_class(c['statements_pct'])}">{c['statements_pct']:.1f}%</td>
                <td class="{cov_class(c['branches_pct'])}">{c['branches_pct']:.1f}%</td>
                <td class="{cov_class(c['functions_pct'])}">{c['functions_pct']:.1f}%</td>
                <td class="{cov_class(c['lines_pct'])}">{c['lines_pct']:.1f}%</td>
            </tr></tbody>
        </table>
    </div>
"""

    def _html_file_coverage_table(self):
        if not self.file_coverage:
            return ""

        def cov_class(pct):
            if pct >= 80: return 'cov-good'
            if pct >= 50: return 'cov-warn'
            return 'cov-bad'

        c = self.coverage
        html = """
    <div class="coverage-section file-coverage-section">
        <h2>File-wise Code Coverage</h2>
        <table class="coverage-table file-cov-table">
            <thead><tr>
                <th class="fc-file-col">File</th>
                <th>% Stmts</th><th>% Branch</th><th>% Funcs</th><th>% Lines</th>
                <th class="fc-uncov-col">Uncovered Line #s</th>
            </tr></thead>
            <tbody>
"""
        html += f"""            <tr class="fc-summary-row">
                <td class="fc-file-cell"><strong>All files</strong></td>
                <td class="{cov_class(c['statements_pct'])}">{c['statements_pct']:.1f}%</td>
                <td class="{cov_class(c['branches_pct'])}">{c['branches_pct']:.1f}%</td>
                <td class="{cov_class(c['functions_pct'])}">{c['functions_pct']:.1f}%</td>
                <td class="{cov_class(c['lines_pct'])}">{c['lines_pct']:.1f}%</td>
                <td class="fc-uncov-cell"></td>
            </tr>
"""

        for fc in self.file_coverage:
            uncovered = fc['uncovered_lines']
            if len(uncovered) == 0:
                uncov_str = ''
            elif len(uncovered) == 1:
                uncov_str = str(uncovered[0])
            else:
                uncov_str = f"{uncovered[0]}-{uncovered[-1]}"
            display_name = fc['filename']
            parts = display_name.replace('\\', '/').rsplit('/', 1)
            if len(parts) == 2:
                dir_part, base = parts
                display_name = f'<span class="fc-dir">{self._esc(dir_part)}/</span>{self._esc(base)}'
            else:
                display_name = self._esc(display_name)

            html += f"""            <tr>
                <td class="fc-file-cell">{display_name}</td>
                <td class="{cov_class(fc['statements_pct'])}">{fc['statements_pct']:.1f}%</td>
                <td class="{cov_class(fc['branches_pct'])}">{fc['branches_pct']:.1f}%</td>
                <td class="{cov_class(fc['functions_pct'])}">{fc['functions_pct']:.1f}%</td>
                <td class="{cov_class(fc['lines_pct'])}">{fc['lines_pct']:.1f}%</td>
                <td class="fc-uncov-cell">{self._esc(uncov_str)}</td>
            </tr>
"""

        html += """            </tbody>
        </table>
    </div>
"""
        return html

    def _html_test_navigation(self, by_file):
        html = '<div class="test-results"><h2>Test Results by Integration File</h2>'

        for file_name in sorted(by_file.keys()):
            tests = by_file[file_name]
            passed = sum(1 for t in tests if t['outcome'] == 'Passed')
            failed = sum(1 for t in tests if t['outcome'] == 'Failed')
            skipped = sum(1 for t in tests if t['outcome'] in ('NotExecuted', 'Inconclusive'))
            safe_id = re.sub(r'[^a-zA-Z0-9]', '_', file_name)

            html += f"""
        <div class="category">
            <div class="category-header" onclick="toggleCategory('{safe_id}')">
                <div>
                    <span class="toggle-icon" id="icon-{safe_id}">&#9654;</span>
                    <span class="category-title">{self._esc(file_name)}</span>
                </div>
                <div class="category-stats">
                    <span class="clr-passed">{passed} passed</span> &middot;
                    <span class="clr-failed">{failed} failed</span> &middot;
                    <span class="clr-skipped">{skipped} skipped</span> &middot;
                    <span>{len(tests)} total</span>
                </div>
            </div>
            <div id="{safe_id}" class="collapsible">
                <table class="test-table">
                    <thead><tr>
                        <th style="width:55%">Test Name</th>
                        <th style="width:20%">Status</th>
                        <th style="width:15%">Duration</th>
                    </tr></thead>
                    <tbody>
"""
            for idx, test in enumerate(tests):
                status_cls = 'status-passed' if test['outcome'] == 'Passed' else 'status-failed' if test['outcome'] == 'Failed' else 'status-skipped'
                icon = '&#9989;' if test['outcome'] == 'Passed' else '&#10060;' if test['outcome'] == 'Failed' else '&#9197;'
                test_id = f"test-{safe_id}-{idx}"

                html += f"""
                        <tr>
                            <td>
                                <div class="test-name" onclick="toggleTestDetails('{test_id}')">{icon} {self._esc(test['name'])}</div>
"""
                detail = self._html_test_detail(test, test_id)
                html += detail
                html += f"""
                            </td>
                            <td><span class="status-badge {status_cls}">{test['outcome']}</span></td>
                            <td>{test['duration']}</td>
                        </tr>
"""
            html += """
                    </tbody>
                </table>
            </div>
        </div>
"""
        html += "</div>"
        return html

    def _html_test_detail(self, test, test_id):
        s = test.get('structured')
        has_error = test['outcome'] == 'Failed' and (test.get('error_message') or test.get('error_stacktrace'))
        has_structured = s and (s.get('assertions') or s.get('requests') or s.get('responses') or s.get('context'))

        if not has_error and not has_structured:
            return ""

        html = f'<div id="{test_id}" class="test-details-container">'

        if has_error:
            html += '<div class="error-details">'
            if test.get('error_message'):
                html += f'<div class="error-message"><strong>Error:</strong><br>{self._esc(test["error_message"])}</div>'
            if test.get('error_stacktrace'):
                html += f"""<details><summary style="cursor:pointer;font-weight:bold;margin-bottom:10px;">Stack Trace</summary>
<div class="error-stacktrace">{self._esc(test["error_stacktrace"])}</div></details>"""
            html += '</div>'

        if not s:
            html += '</div>'
            return html

        if s.get('assertions'):
            html += '<div class="test-details-section"><h4 class="details-heading">Assertions</h4><div class="assertions-table">'
            for a in s['assertions']:
                icon = '&#9989;' if a.get('passed', True) else '&#10060;'
                row_cls = '' if a.get('passed', True) else 'a-failed'
                html += f"""
<div class="assertion-row {row_cls}">
    <div class="assertion-header"><span class="assertion-icon">{icon}</span><strong>{self._esc(a['name'])}</strong></div>
    <div class="expected-actual-row">
        <div><span class="ea-label">Expected:</span><pre class="value-box">{self._esc(str(a['expected']))}</pre></div>
        <div><span class="ea-label">Actual:</span><pre class="value-box">{self._esc(str(a['actual']))}</pre></div>
    </div>
</div>"""
            html += '</div></div>'

        requests = s.get('requests', [])
        responses = s.get('responses', [])
        pairs = max(len(requests), len(responses))
        if pairs > 0:
            html += '<div class="test-details-section"><h4 class="details-heading">HTTP Transactions</h4>'
            for i in range(pairs):
                req = requests[i] if i < len(requests) else None
                res = responses[i] if i < len(responses) else None

                if req:
                    sdk_badge = ''
                    if req.get('sdkMethod'):
                        sdk_badge = f'<div class="sdk-method-badge">SDK Method: <code>{self._esc(req["sdkMethod"])}</code></div>'
                    html += f"""
<div class="request-block">
    {sdk_badge}
    <div class="request-summary"><span class="http-method">{self._esc(req['method'])}</span><span class="http-url">{self._esc(req['url'])}</span></div>"""
                    if req.get('headers'):
                        hdr_text = '\n'.join(f"{k}: {v}" for k, v in req['headers'].items())
                        html += f"""
    <details class="req-details"><summary>Request Headers</summary><pre class="headers-box">{self._esc(hdr_text)}</pre></details>"""
                    if req.get('body'):
                        html += f"""
    <details class="req-details"><summary>Request Body</summary><pre class="body-box">{self._esc(req['body'][:5000])}</pre></details>"""
                    if req.get('curl'):
                        curl_id = f"curl-{test_id}-{i}"
                        html += f"""
    <details class="req-details curl-section"><summary>cURL Command</summary>
        <pre class="curl-box" id="{curl_id}-pre">{self._esc(req['curl'])}</pre>
        <button class="copy-btn" onclick="copyToClipboard('{curl_id}-pre')">Copy</button>
    </details>"""
                    html += '</div>'

                if res:
                    sc = res.get('statusCode', 0)
                    status_cls = 'rs-success' if 200 <= sc < 300 else 'rs-error'
                    html += f"""
<div class="response-block">
    <div class="response-summary {status_cls}"><span class="http-status">{sc} {self._esc(res.get('statusText', ''))}</span></div>"""
                    if res.get('headers'):
                        hdr_text = '\n'.join(f"{k}: {v}" for k, v in res['headers'].items())
                        html += f"""
    <details class="res-details"><summary>Response Headers</summary><pre class="headers-box">{self._esc(hdr_text)}</pre></details>"""
                    if res.get('body'):
                        body_text = res['body']
                        truncated = len(body_text) > 3000
                        display_body = body_text[:3000] if truncated else body_text
                        try:
                            parsed = json.loads(display_body)
                            display_body = json.dumps(parsed, indent=2)[:3000]
                        except (json.JSONDecodeError, ValueError):
                            pass
                        body_id = f"resbody-{test_id}-{i}"
                        html += f"""
    <details class="res-details"><summary>Response Body</summary>
        <pre class="body-box" id="{body_id}">{self._esc(display_body)}</pre>"""
                        if truncated:
                            html += f'<button class="show-more-btn" onclick="showFullBody(this, \'{body_id}\')">Show More</button>'
                            html += f'<script>document.getElementById("{body_id}").dataset.full = {json.dumps(self._esc(body_text))};</script>'
                        html += '</details>'
                    html += '</div>'
            html += '</div>'

        if s.get('context'):
            html += """
<details class="test-details-section" style="margin-bottom:15px;">
    <summary class="details-heading" style="cursor:pointer;">Test Context</summary>
    <table style="width:100%;border-collapse:collapse;margin-top:10px;font-size:0.85em;"><tbody>"""
            for ctx in s['context']:
                html += f"""
        <tr style="border-bottom:1px solid #e9ecef;">
            <td style="padding:6px 10px;font-weight:600;white-space:nowrap;color:#555;width:180px;">{self._esc(ctx['key'])}</td>
            <td style="padding:6px 10px;font-family:Consolas,Monaco,monospace;word-break:break-all;">{self._esc(str(ctx['value']))}</td>
        </tr>"""
            html += '</tbody></table></details>'

        html += '</div>'
        return html

    def _html_footer(self):
        now = datetime.now().strftime('%Y-%m-%d at %H:%M:%S')
        return f"""
    <div class="footer">
        <p><strong>.NET CMA SDK &mdash; Integration Test Report</strong></p>
        <p>Generated on {now}</p>
        <p>Report includes: Test Results (TRX) + Code Coverage (Cobertura) + HTTP Transactions + Assertions</p>
    </div>
"""

    def _html_scripts(self):
        return """
    <script>
        function toggleCategory(categoryId) {
            const el = document.getElementById(categoryId);
            const icon = document.getElementById('icon-' + categoryId);
            if (el.classList.contains('active')) {
                el.classList.remove('active');
                icon.classList.remove('rotated');
            } else {
                el.classList.add('active');
                icon.classList.add('rotated');
            }
        }
        function toggleTestDetails(testId) {
            const el = document.getElementById(testId);
            if (el) el.classList.toggle('show');
        }
        function copyToClipboard(elementId) {
            const el = document.getElementById(elementId);
            if (el) {
                const text = el.textContent || el.innerText;
                navigator.clipboard.writeText(text).then(() => {
                    const btn = el.parentElement.querySelector('.copy-btn');
                    if (btn) { const orig = btn.textContent; btn.textContent = 'Copied!'; setTimeout(() => btn.textContent = orig, 2000); }
                });
            }
        }
        function showFullBody(btn, elementId) {
            const el = document.getElementById(elementId);
            if (el && el.dataset.full) {
                el.textContent = el.dataset.full;
                btn.style.display = 'none';
            }
        }
        document.addEventListener('DOMContentLoaded', function() {
            document.querySelectorAll('.category').forEach(cat => {
                const stats = cat.querySelector('.category-stats');
                if (stats && stats.textContent.includes('failed') && !stats.textContent.includes('0 failed')) {
                    const coll = cat.querySelector('.collapsible');
                    if (coll) toggleCategory(coll.id);
                }
            });
        });
    </script>
"""


def _validate_input_path(file_path, description="file"):
    """
    Validate input file paths to prevent path traversal attacks.
    """
    if not file_path or not isinstance(file_path, str):
        raise ValueError(f"Invalid {description} path: path must be a non-empty string")
    
    # Check for null bytes
    if '\x00' in file_path:
        raise ValueError(f"Invalid {description} path: contains null byte")
    
    # Resolve and validate the path
    try:
        resolved_path = os.path.abspath(os.path.normpath(file_path))
    except (OSError, ValueError) as e:
        raise ValueError(f"Invalid {description} path: cannot resolve path: {e}") from e
    
    # Check if file exists and is readable
    if not os.path.exists(resolved_path):
        raise ValueError(f"{description.capitalize()} not found: {resolved_path}")
    
    if not os.path.isfile(resolved_path):
        raise ValueError(f"{description.capitalize()} is not a regular file: {resolved_path}")
    
    if not os.access(resolved_path, os.R_OK):
        raise ValueError(f"{description.capitalize()} is not readable: {resolved_path}")
    
    return resolved_path


def main():
    parser = argparse.ArgumentParser(description='Integration Test Report Generator for .NET CMA SDK')
    parser.add_argument('trx_file', help='Path to the .trx test results file')
    parser.add_argument('--coverage', help='Path to coverage.cobertura.xml file', default=None)
    parser.add_argument('--output', help='Output HTML file path', default=None)
    args = parser.parse_args()

    try:
        # Validate input file paths
        trx_file = _validate_input_path(args.trx_file, "TRX file")
        coverage_file = None
        if args.coverage:
            coverage_file = _validate_input_path(args.coverage, "coverage file")
    except ValueError as e:
        print(f"Error: {e}")
        sys.exit(1)

    print("=" * 70)
    print("  .NET CMA SDK - Integration Test Report Generator")
    print("=" * 70)

    generator = IntegrationTestReportGenerator(trx_file, coverage_file)

    print(f"\nParsing TRX: {trx_file}")
    generator.parse_trx()
    print(f"  Found {generator.results['total']} integration tests")
    print(f"    Passed:  {generator.results['passed']}")
    print(f"    Failed:  {generator.results['failed']}")
    print(f"    Skipped: {generator.results['skipped']}")

    if coverage_file:
        print(f"\nParsing Coverage: {coverage_file}")
        generator.parse_coverage()
        c = generator.coverage
        print(f"  Lines:      {c['lines_pct']:.1f}%")
        print(f"  Branches:   {c['branches_pct']:.1f}%")
        print(f"  Functions:  {c['functions_pct']:.1f}%")

    timestamp = datetime.now().strftime('%Y%m%d_%H%M%S')
    output_file = args.output or f'integration-test-report_{timestamp}.html'

    print(f"\nGenerating HTML report...")
    try:
        resolved_output = generator.generate_html(output_file)
    except ValueError as e:
        print(f"Error: {e}")
        sys.exit(1)

    print(f"\n{'=' * 70}")
    print(f"  Report generated: {resolved_output}")
    print(f"{'=' * 70}")
    print(f"\n  open {resolved_output}")


if __name__ == "__main__":
    main()
