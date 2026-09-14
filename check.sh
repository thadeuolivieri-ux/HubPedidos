#!/bin/bash
set -e

echo "=== 1. Restauração de Pacotes ==="
dotnet restore

echo "=== 2. Compilação da Solução ==="
dotnet build --no-restore

echo "=== 3. Verificação de Formatação de Código ==="
dotnet format --verify-no-changes

echo "=== 4. Execução da Suíte de Testes (31 Testes) ==="
dotnet test --no-build
