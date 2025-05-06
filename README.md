# Sistema de Rastreabilidade Animal


## 📌 Visão Geral

Solução desenvolvida em .NET 8 que:
- Gerencia unidades de exploração (UEPs) e suas características
- Controla movimentações de animais entre propriedades
- Garante conformidade com regras sanitárias e de negócio
- Previne erros operacionais com validações automatizadas

## 🏗️ Componentes Principais

### Entidades

#### Unidade de Exploração (UEP)
- `ID` único
- Código de espécie
- Quantidade de animais
- Código da propriedade
- Relacionamento com saídas

#### Saída de Animais
- `ID` único
- Data da movimentação
- UEP de origem
- UEP de destino
- Quantidade de animais

## ⚙️ Regras de Negócio

1. **Validações obrigatórias**:
   - Origem ≠ Destino
   - Quantidade ≤ Estoque disponível na origem
   - Espécie compatível entre UEPs

2. **Operações automáticas**:
   - Atualização de estoques nas UEPs
   - Registro de histórico completo

## 🚀 Funcionalidades

### Para Unidades de Exploração (UEP)
| Endpoint | Método | Descrição |
|----------|--------|-----------|
| `/api/unidadeexploracao` | GET | Lista todas UEPs |
| `/api/unidadeexploracao/{id}` | GET | Busca UEP específica |
| `/api/unidadeexploracao` | POST | Cadastra nova UEP |
| `/api/unidadeexploracao/{id}` | PUT | Atualiza quantidade de animais |
| `/api/unidadeexploracao/{id}` | DELETE | Remove UEP |

### Para Saídas de Animais
| Endpoint | Método | Descrição |
|----------|--------|-----------|
| `/api/saidaanimais` | GET | Lista todas movimentações |
| `/api/saidaanimais/{id}` | GET | Busca movimentação específica |
| `/api/saidaanimais` | POST | Registra nova saída |
| `/api/saidaanimais/{id}` | PUT | Atualiza dados da movimentação |
| `/api/saidaanimais/{id}` | DELETE | Remove registro e ajusta estoques |


## 🛠️ Tecnologias Utilizadas

- .NET 8
- Entity Framework Core
- FluentValidation
- xUnit (testes)
- Swagger (documentação)
- Moq (mocking)

