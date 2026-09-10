using ApiBling.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiBling.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Produto> Produtos { get; set; }
        public DbSet<BlingToken> BlingTokens { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidoItens { get; set; }
        public DbSet<NotaFiscal> NotasFiscais { get; set; }
        public DbSet<NotaFiscalItem> NotaFiscalItens { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<NaturezaOperacao> NaturezasOperacoes => Set<NaturezaOperacao>();
        public DbSet<NotaFiscalGeral> NotasFiscaisGerais { get; set; } = null!;
        public DbSet<NotaFiscalGeralItem> NotasFiscaisGeraisItens { get; set; }
        public DbSet<SituacaoPedidoVenda> SituacoesPedidosVenda { get; set; }
        public DbSet<NotaFiscalServico> NotasFiscaisServico { get; set; }
        public DbSet<SituacaoNotaFiscalServico> SituacoesNotasFiscaisServico { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações da tabela Produto
            modelBuilder.Entity<Produto>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Cria um índice no BlingId para deixar as buscas de UPDATE muito mais rápidas
                entity.HasIndex(e => e.BlingId).IsUnique();

                entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Preco).HasPrecision(18, 4);
                entity.Property(e => e.PrecoCusto).HasPrecision(18, 4);
                entity.Property(e => e.Estoque).HasPrecision(18, 4);
                entity.Property(p => p.IdCategoriaBling).IsRequired(false);
            });

            // Configurações da tabela BlingToken
            modelBuilder.Entity<BlingToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AccessToken).IsRequired();
                entity.Property(e => e.RefreshToken).IsRequired();
            });

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.BlingId).IsUnique();
                entity.Property(e => e.NomeCliente).HasMaxLength(200);
                entity.Property(e => e.Total).HasPrecision(18, 4);
                entity.Property(e => e.TotalProdutos).HasPrecision(18, 4);
                entity.Property(e => e.ValorFrete).HasPrecision(18, 4);
                entity.Property(e => e.ValorDesconto).HasPrecision(18, 4);
            });

            modelBuilder.Entity<PedidoItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantidade).HasPrecision(18, 4);
                entity.Property(e => e.Valor).HasPrecision(18, 4);

                // Configura a relação 1 para N (1 Pedido tem Vários Itens)
                entity.HasOne(e => e.Pedido)
                      .WithMany(p => p.Itens)
                      .HasForeignKey(e => e.PedidoId);
            });

            modelBuilder.Entity<NotaFiscal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.BlingId).IsUnique();

                // Evita o aviso amarelo do Entity Framework
                entity.Property(e => e.ValorNota).HasPrecision(18, 4);
                entity.Property(e => e.ValorFrete).HasPrecision(18, 4);
            });

            modelBuilder.Entity<NotaFiscalItem>(entity =>
            {
                entity.HasKey(e => e.Id);

             

                // Configura o relacionamento 1 para N
                entity.HasOne(e => e.NotaFiscal)
                      .WithMany(n => n.Itens)
                      .HasForeignKey(e => e.NotaFiscalId);
            });

            modelBuilder.Entity<NotaFiscalItem>(entity =>
            {
                entity.Property(e => e.Quantidade).HasPrecision(18, 4);
                entity.Property(e => e.Valor).HasPrecision(18, 2);
                entity.Property(e => e.ValorTotal).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("Categorias");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.IdBling).IsRequired();
                entity.Property(c => c.Nome).IsRequired().HasMaxLength(255);
                entity.Property(c => c.Tipo).HasMaxLength(50);
                entity.Property(c => c.IdCategoriaPai);
                entity.Property(c => c.DataAtualizacao).IsRequired();
            });

            modelBuilder.Entity<NaturezaOperacao>(entity =>
            {
                entity.ToTable("naturezas_operacoes");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
                entity.Property(e => e.Descricao).HasColumnName("descricao").HasMaxLength(255).IsRequired();
                entity.Property(e => e.Tipo).HasColumnName("tipo").HasMaxLength(50);
                entity.Property(e => e.DataCriacao).HasColumnName("data_criacao");
                entity.Property(e => e.DataAlteracao).HasColumnName("data_alteracao");
            });

            modelBuilder.Entity<NotaFiscalGeral>(entity =>
            {
                entity.ToTable("notas_fiscais_gerais");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .HasColumnName("id")
                      .ValueGeneratedNever();

                entity.Property(e => e.Numero).HasColumnName("numero").IsRequired();
                entity.Property(e => e.ChaveAcesso).HasColumnName("chave_acesso").HasMaxLength(64);
                entity.Property(e => e.Tipo).HasColumnName("tipo").HasMaxLength(10);
                entity.Property(e => e.Situacao).HasColumnName("situacao").HasMaxLength(30);
                entity.Property(e => e.IdNaturezaOperacao).HasColumnName("natureza_operacao").HasMaxLength(120);
                entity.Property(e => e.DataEmissao).HasColumnName("data_emissao");
                entity.Property(e => e.DataOperacao).HasColumnName("data_operacao");
                entity.Property(e => e.IdLoja).HasColumnName("Id_Loja").HasMaxLength(64);

            });

            modelBuilder.Entity<NotaFiscalGeralItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.NotaFiscalGeral)
                      .WithMany(n => n.Itens)
                      .HasForeignKey(e => e.IdNotaFiscalGeral)
                      .OnDelete(DeleteBehavior.Cascade); // Se apagar a nota, apaga os itens
            });

            modelBuilder.Entity<SituacaoPedidoVenda>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever(); // O ID vem do Bling
            });

            modelBuilder.Entity<NotaFiscalServico>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever(); // O ID vem do Bling
                entity.Property(e => e.Valor).HasPrecision(18, 4); // Evita o warning do Entity Framework
            });

            modelBuilder.Entity<SituacaoNotaFiscalServico>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever(); // O ID é fixo, não é Identity
                entity.Property(e => e.Nome).HasMaxLength(50).IsRequired();

                // INSERE OS DADOS FIXOS NO BANCO AUTOMATICAMENTE
                entity.HasData(
                    new SituacaoNotaFiscalServico { Id = 0, Nome = "Pendente" },
                    new SituacaoNotaFiscalServico { Id = 1, Nome = "Emitida" },
                    new SituacaoNotaFiscalServico { Id = 2, Nome = "Disponível para consulta" },
                    new SituacaoNotaFiscalServico { Id = 3, Nome = "Cancelada" }
                );
            });

            // Configura a Chave Estrangeira na tabela de Notas de Serviço
            modelBuilder.Entity<NotaFiscalServico>(entity =>
            {
                entity.HasOne(e => e.SituacaoObj)
                      .WithMany(s => s.NotasFiscais)
                      .HasForeignKey(e => e.Situacao)
                      .OnDelete(DeleteBehavior.Restrict); // Não deixa apagar uma situação se tiver nota usando ela
            });



        }
    }
}
