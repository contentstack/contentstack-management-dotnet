#!/bin/bash
set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
TEST_PROJECT="Contentstack.Management.Core.Tests"

echo "======================================================"
echo "  CMA SDK — Integration Test Report Generator"
echo "======================================================"
echo ""
echo "Project: $PROJECT_ROOT"
echo "Run ID:  $TIMESTAMP"
echo ""

# Step 1: Run ONLY integration tests, collect TRX + coverage
TRX_FILE="IntegrationTest-Report-${TIMESTAMP}.trx"
echo "Step 1: Running integration tests..."
dotnet test "$PROJECT_ROOT/$TEST_PROJECT/$TEST_PROJECT.csproj" \
  --filter "FullyQualifiedName~IntegrationTest" \
  --logger "trx;LogFileName=$TRX_FILE" \
  --results-directory "$PROJECT_ROOT/$TEST_PROJECT/TestResults" \
  --collect:"XPlat code coverage" \
  --verbosity quiet || true

echo ""
echo "Tests completed."
echo ""

# Step 2: Locate the cobertura coverage file (most recent)
COBERTURA=""
if [ -d "$PROJECT_ROOT/$TEST_PROJECT/TestResults" ]; then
  COBERTURA=$(find "$PROJECT_ROOT/$TEST_PROJECT/TestResults" \
    -name "coverage.cobertura.xml" 2>/dev/null | sort -r | head -1)
fi

TRX_PATH="$PROJECT_ROOT/$TEST_PROJECT/TestResults/$TRX_FILE"
echo "TRX:      $TRX_PATH"
echo "Coverage: ${COBERTURA:-Not found}"
echo ""

# Step 3: Generate the HTML report
echo "Step 2: Generating HTML report..."
cd "$PROJECT_ROOT"

COVERAGE_ARG=""
if [ -n "$COBERTURA" ]; then
  COVERAGE_ARG="--coverage $COBERTURA"
fi

OUTPUT_FILE="$PROJECT_ROOT/integration-test-report_${TIMESTAMP}.html"

python3 "$PROJECT_ROOT/Scripts/generate_integration_test_report.py" \
  "$TRX_PATH" \
  $COVERAGE_ARG \
  --output "$OUTPUT_FILE"

echo ""
echo "======================================================"
echo "  All Done!"
echo "======================================================"
echo ""
if [ -f "$OUTPUT_FILE" ]; then
  echo "Report: $OUTPUT_FILE"
  echo ""
  echo "To open:  open $OUTPUT_FILE"
else
  echo "Warning: Report file not found. Check output above for errors."
fi
echo ""
