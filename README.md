# api-gateway

Nesse projeto como um todo foi montado varios micro serviços resolvi deixar aqui a 
documentação de como funciona o fluxo dos microserviços e o que foi utilizado.


1️⃣ Usuário inicia ação

O usuário interage com o API Gateway:

Login / cadastro → Users Microservice

Compra de jogo → Games Microservice

O Gateway valida o JWT do usuário e roteia para o microserviço correto.

2️⃣ Games Microservice

Recebe a requisição de compra do usuário:

Endpoint: POST /api/games/{gameId}/purchase

Cria um PurchaseGameCommand via MediatR.

Registro de compra:

Marca jogo como comprado na entidade Game.

Publica evento GamePurchasedIntegrationEvent no RabbitMQ.

Também envia dados para o ElasticSearch:

Indexa o jogo e informações de compra (opcional para recomendações).

3️⃣ RabbitMQ

Broker de mensagens com Topic Exchange.

Eventos:

game.purchased → consumido por Payments.

payment.completed → consumido por Games.

Outros eventos como user.created → consumidos por Lambda (opcional).

Cada fila tem consumer dedicado que processa os eventos.

4️⃣ Payments Microservice

Consome game.purchased:

Cria um Payment com status Pending.

Processa o pagamento (simulado ou real).

Se o pagamento for aprovado → publica PaymentCompletedIntegrationEvent.

Se o pagamento falhar → publica PaymentFailedIntegrationEvent.

Persistência:

Salva o Payment no SQL Server do Payments Microservice.

5️⃣ Games Microservice (consumidor de pagamento)

Consome payment.completed:

Atualiza o Game para registrar a compra.

Incrementa contador de Purchases.

Atualiza índice no ElasticSearch (games).

Esse índice será usado para:

Search → busca por nome, categoria.

Recommendations → sugere jogos com base nas categorias mais compradas pelo usuário.

6️⃣ ElasticSearch

Índices:

games → jogos, categoria, preço, número de compras.

user-purchases (opcional) → histórico de compras do usuário.

Consultas usadas:

search → busca por termo (nome/categoria).

recommendations → busca baseada nas categorias mais compradas pelo usuário.

7️⃣ Users Microservice

Fornece dados do usuário (perfil, histórico) para recomendações.

Pode ser consumido via API diretamente ou via eventos (event sourcing opcional).

8️⃣ Observabilidade

Jaeger:

Captura traces distribuídos das requisições e eventos.

Prometheus:

Captura métricas de APIs, RabbitMQ, ElasticSearch.

Grafana:

Dashboards para monitoramento de latência, erros, filas e métricas do sistema.

9️⃣ Fluxo completo de uma compra

Usuário chama API Gateway → Games Microservice /purchase.

Games registra compra localmente e publica game.purchased.

Payments consome game.purchased, processa pagamento.

Se aprovado → Payments publica payment.completed.

Games consome payment.completed, atualiza compras e ElasticSearch.

Usuário agora vê que o jogo foi comprado e pode aparecer nas recomendações.
