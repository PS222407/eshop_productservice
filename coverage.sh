#!/usr/bin/env bash
rm -r xplat/coverage-report/*
dotnet test --settings coverlet.runsettings --collect:"XPlat Code Coverage" --results-directory:"xplat/xml"
mv ./xplat/xml/*/coverage.cobertura.xml ./xplat/xml 2>/dev/null || true
reportgenerator -reports:xplat/xml/coverage.cobertura.xml -targetdir:xplat/coverage-report -reporttypes:Html && xdg-open xplat/coverage-report/index.html
rm -r xplat/xml/*
