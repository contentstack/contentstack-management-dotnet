#!/bin/sh

#  run-test-case.sh
#  Contentstack
#
#  Created by Uttam Ukkoji on 12/04/21.
#  Copyright © 2020 Contentstack. All rights reserved.

cp -a docfx_project docs

mkdir docs/src

cp -a Contentstack.Management.Core docs/src/Contentstack.Management.Core

docfx docs/docfx.json

cp -a docs/_site api_referece

rm -rf docs

open api_referece/index.html
