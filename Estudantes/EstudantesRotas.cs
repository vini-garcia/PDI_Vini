using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using WebAPIVini.Estudantes.Data;

namespace WebAPIVini.Estudantes
{
    public static class EstudantesRotas
    {
        public static async void AddRotasEstudantes(this WebApplication app)
        {
            var rotasEstudantes = app.MapGroup("estudantes");

            rotasEstudantes
                .MapPost(
                    "",
                    async (
                        AddEstudanteRequest request,
                        AppDbContext context,
                        CancellationToken ct
                    ) =>
                    {
                        var estudanteJaExiste = await context.Estudantes.AnyAsync(
                            estudante => estudante.Nome == request.Nome,
                            ct
                        );

                        if (estudanteJaExiste)
                            return Results.Conflict("Estudante já cadastrado");

                        var novoEstudante = new Estudante(request.Nome);
                        await context.Estudantes.AddAsync(novoEstudante, ct);
                        await context.SaveChangesAsync(ct);

                        var estudanteRetorno = new EstudanteDTO(
                            novoEstudante.Id,
                            novoEstudante.Nome
                        );

                        return Results.Ok(estudanteRetorno);
                    }
                )
                .WithMetadata(
                    new SwaggerOperationAttribute(
                        summary: "Adiciona um novo estudante",
                        description: "Esta rota adiciona um novo estudante ao banco de dados."
                    )
                )
                .Produces<EstudanteDTO>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status409Conflict);

            rotasEstudantes
                .MapGet(
                    "/ativos",
                    async (AppDbContext context, CancellationToken ct) =>
                    {
                        var estudantes = await context
                            .Estudantes.Where(estudante => estudante.Ativo)
                            .Select(estudante => new EstudanteDTO(estudante.Id, estudante.Nome))
                            .ToListAsync(ct);

                        return estudantes;
                    }
                )
                .WithMetadata(
                    new SwaggerOperationAttribute(
                        summary: "Obtém todos os estudantes ativos",
                        description: "Esta rota retorna uma lista de todos os estudantes que estão marcados como ativos no banco de dados."
                    )
                )
                .Produces<List<EstudanteDTO>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError);

            rotasEstudantes
                .MapGet(
                    "/inativos",
                    async (AppDbContext context, CancellationToken ct) =>
                    {
                        var estudantes = await context
                            .Estudantes.Where(estudante => !estudante.Ativo)
                            .Select(estudante => new EstudanteDTO(estudante.Id, estudante.Nome))
                            .ToListAsync(ct);

                        return estudantes;
                    }
                )
                .WithMetadata(
                    new SwaggerOperationAttribute(
                        summary: "Obtém todos os estudantes inativos",
                        description: "Esta rota retorna uma lista de todos os estudantes que não estão marcados como ativos no banco de dados."
                    )
                )
                .Produces<List<EstudanteDTO>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError);

            rotasEstudantes
                .MapGet(
                    "",
                    async (AppDbContext context, CancellationToken ct) =>
                    {
                        var estudantes = await context
                            .Estudantes.Select(estudante => new EstudanteDTO(
                                estudante.Id,
                                estudante.Nome
                            ))
                            .ToListAsync(ct);

                        return estudantes;
                    }
                )
                .WithMetadata(
                    new SwaggerOperationAttribute(
                        summary: "Obtém todos os estudantes, ativos e inativos",
                        description: "Esta rota retorna uma lista de todos os estudantes, ativos e inativos, no banco de dados."
                    )
                )
                .Produces<List<EstudanteDTO>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status500InternalServerError);

            rotasEstudantes
                .MapPut(
                    "{id:guid}",
                    async (
                        Guid id,
                        UpdateEstudanteRequest request,
                        AppDbContext context,
                        CancellationToken ct
                    ) =>
                    {
                        var estudante = await context.Estudantes.SingleOrDefaultAsync(
                            estudante => estudante.Id == id,
                            ct
                        );

                        if (estudante == null)
                            return Results.NotFound();

                        estudante.AtualizarNome(request.Nome);

                        await context.SaveChangesAsync(ct);

                        return Results.Ok(new EstudanteDTO(estudante.Id, estudante.Nome));
                    }
                )
                .WithMetadata(
                    new SwaggerOperationAttribute(
                        summary: "Atualiza um estudante existente",
                        description: "Esta rota atualiza o nome de um estudante existente no banco de dados, identificado pelo seu ID."
                    )
                )
                .Produces<EstudanteDTO>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);

            rotasEstudantes
                .MapDelete(
                    "{id:guid}",
                    async (Guid id, AppDbContext context, CancellationToken ct) =>
                    {
                        var estudante = await context.Estudantes.SingleOrDefaultAsync(
                            estudante => estudante.Id == id,
                            ct
                        );

                        if (estudante == null)
                            return Results.NotFound();

                        estudante.Desativar();

                        await context.SaveChangesAsync(ct);

                        return Results.Ok();
                    }
                )
                .WithMetadata(
                    new SwaggerOperationAttribute(
                        summary: "Desativa um estudante existente",
                        description: "Esta rota desativa um estudante existente no banco de dados, identificado pelo seu ID."
                    )
                )
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status500InternalServerError);
        }
    }
}
