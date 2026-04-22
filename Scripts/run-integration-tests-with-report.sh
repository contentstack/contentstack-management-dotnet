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

if ! command -v dotnet >/dev/null 2>&1; then
  echo "Error: 'dotnet' was not found on your PATH."
  echo "Install the .NET SDK (same major version as the test projects) and open a new terminal, or see:"
  echo "  https://dotnet.microsoft.com/download"
  exit 1
fi

if ! command -v python3 >/dev/null 2>&1; then
  echo "Error: 'python3' was not found on your PATH (required for the HTML report)."
  exit 1
fi

SETTINGS="$PROJECT_ROOT/$TEST_PROJECT/appsettings.json"
if [ ! -f "$SETTINGS" ]; then
  echo "Error: Integration tests require credentials at:"
  echo "  $SETTINGS"
  echo "Copy the template and fill in real values (do not commit secrets):"
  echo "  cp \"$PROJECT_ROOT/$TEST_PROJECT/appsettings.json.example\" \"$SETTINGS\""
  exit 1
fi

# Step 1: Run ONLY integration tests, collect TRX + coverage
TRX_FILE="IntegrationTest-Report-${TIMESTAMP}.trx"
TRX_PATH="$PROJECT_ROOT/$TEST_PROJECT/TestResults/$TRX_FILE"
echo "Step 1: Running integration tests..."
set +e
dotnet test "$PROJECT_ROOT/$TEST_PROJECT/$TEST_PROJECT.csproj" \
  --filter "FullyQualifiedName~IntegrationTest" \
  --logger "trx;LogFileName=$TRX_FILE" \
  --results-directory "$PROJECT_ROOT/$TEST_PROJECT/TestResults" \
  --collect:"XPlat code coverage" \
  --verbosity quiet
TEST_EXIT=$?
set -e

echo ""
echo "Tests completed (dotnet exit code: $TEST_EXIT)."
echo ""

if [ ! -f "$TRX_PATH" ]; then
  echo "Error: TRX file was not created at:"
  echo "  $TRX_PATH"
  echo "Fix the dotnet/test errors above (or install the SDK), then run this script again."
  exit 1
fi

# Step 2: Locate the cobertura coverage file (most recent)
COBERTURA=""
if [ -d "$PROJECT_ROOT/$TEST_PROJECT/TestResults" ]; then
  COBERTURA=$(find "$PROJECT_ROOT/$TEST_PROJECT/TestResults" \
    -name "coverage.cobertura.xml" 2>/dev/null | sort -r | head -1)
fi

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
