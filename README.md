# Sistema de Gestão de Estacionamento

Este repositório contém a implementação das melhorias e correções solicitadas no desafio de código.  
O sistema permite o gerenciamento de **clientes, veículos, mensalistas e faturamento**, além de importação de dados via **CSV**.

---

## Decisões Técnicas

### 1. Organização e Clareza do Código
- Separação das camadas respeitando boas práticas: Controllers, Services e Repositórios.
- Centralização de regras de negócio no **FaturamentoService** para reduzir duplicidade e melhorar a orquestração.
- Uso de **DTOs** para controlar o tráfego de dados entre frontend e backend, evitando exposição direta das entidades.

### 2. Banco de Dados e ORM
- Uso do **Entity Framework** com PostgreSQL.
- Criação de **índice único composto** em `(Nome, Telefone)` para garantir unicidade de clientes e evitar inconsistências.
- Ajustes nas queries para maior eficiência e segurança, utilizando LINQ com tratamento de cenários de valores nulos.

### 3. Faturamento Parcial
- Implementação da lógica proporcional considerando **data de inclusão** e **vigência** de veículos.
- Cálculo do valor diário da mensalidade baseado no número de dias do mês (`valorMensalidade / diasNoMes`).
- Exemplo:
  - Cliente A (01 a 10/09) → fatura proporcional a **10 dias**.
  - Cliente B (11 a 30/09) → fatura proporcional a **20 dias**.

### 4. Upload CSV
- Melhoria no feedback de erros:
  - Exibição da **linha exata** e do **motivo do erro**.
  - Exemplo: `Linha 12 - Telefone inválido`.
- O relatório de importação foi ajustado para ser mais informativo e orientar o usuário na correção.

### 5. Frontend (React)
- Inclusão de **edição de clientes e veículos** com formulários reativos.
- Implementação de **validações no input** (exemplo: formato de competência `yyyy-MM`).
- Exibição de **mensagens de erro claras** para o usuário.
- Melhor experiência de uso na listagem, edição e importação de dados.

### 6. Integração Frontend ↔ Backend
- Uso do **React Query** para cache e revalidação de dados.
- Tratamento de erros com `try/catch` e `isError` do React Query.
- Requisições assíncronas com atualização imediata da tela após operações de edição ou geração de fatura.

---

## Funcionalidades Implementadas

- [x] **Edição de Clientes**
  - Nome, Telefone, Endereço, Valor da mensalidade, Mensalista.
  - Validação de unicidade `(Nome + Telefone)`.

- [x] **Edição de Veículos**
  - Alterar Modelo, Ano e Cliente associado.

- [x] **Upload CSV**
  - Relatório detalhado de erros com linha e motivo.

- [x] **Faturamento Parcial**
  - Cálculo proporcional baseado na vigência do cliente/veículo.

- [x] **Mensagens de Erro no Frontend**
  - Exibição clara de feedback ao usuário.

---

## Tecnologias

**Backend**
- .NET 8
- Entity Framework Core
- PostgreSQL

**Frontend**
- React
- React Query
- Axios
- Vite

---

## Como Executar
#### Banco PostgreSQL
1. Crie um banco local (ex.: `parking_test`) e ajuste a `ConnectionString` em `appsettings.json`, se necessário.  


### Backend
```bash
cd backend
dotnet restore
dotnet ef database update
dotnet run

A API será iniciada (por padrão) em `http://localhost:5000`. Swagger ativado em `/swagger`.  

#### Frontend
```bash
cd src/frontend
npm install
npm run dev
```
A aplicação ficará disponível em `http://localhost:5173`.  
Configure `VITE_API_URL` caso seja necessário apontar para outra porta.  

### 4.3 Estrutura de Pastas
```
/src/backend        -> API .NET 8
/src/frontend       -> React (Vite)
/scripts/seed.sql   -> Criação e seed do banco
/scripts/exemplo.csv-> CSV de exemplo
```

```

