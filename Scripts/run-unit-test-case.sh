#!/bin/sh

#  run-unit-test-case.sh
#  Contentstack
#
#  Created by Uttam Ukkoji on 30/03/2023.
#  Copyright © 2023 Contentstack. All rights reserved.

echo "Removing files"
rm -rf "./Contentstack.Management.Core.Unit.Tests/TestResults"

FILE_NAME="Contentstack-DotNet-Test-Case"

echo "Running test case..."
dotnet test "Contentstack.Management.Core.Unit.Tests/Contentstack.Management.Core.Unit.Tests.csproj" --logger "trx;LogFileName=Report-$FILE_NAME.trx" --collect:"XPlat code coverage"

echo "Test case Completed..."   
