namespace HubPedidos.Application.Data;

using Microsoft.EntityFrameworkCore;
using HubPedidos.Domain.Entities;
using HubPedidos.Domain.ValueObjects;

public class HubPedidosDbContext : DbContext
{
    public DbSet<Pedido> Pedidos => Set<Pedido>();

    public HubPedidosDbContext(DbContextOptions<HubPedidosDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pedido>(builder =>
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasConversion(id => id.Value, value => new PedidoId(value))
                .IsRequired();

            builder.Property(p => p.Regiao).IsRequired().HasMaxLength(50);
            builder.Property(p => p.Prioridade).IsRequired();

            builder.HasIndex(p => p.Regiao);

            // Mapeamento de Tipo Complexo / Owned Type para Endereço
            builder.OwnsOne(p => p.EnderecoEntrega, end =>
            {
                end.Property(e => e.Logradouro).HasColumnName("Endereco_Logradouro").HasMaxLength(150);
                end.Property(e => e.Cidade).HasColumnName("Endereco_Cidade").HasMaxLength(100);
                end.Property(e => e.Estado).HasColumnName("Endereco_Estado").HasMaxLength(2);
                end.Property(e => e.Cep).HasColumnName("Endereco_Cep").HasMaxLength(10);
            });

            // Mapeamento da coleção de ItemPedido
            builder.OwnsMany(p => p.Itens, item =>
            {
                item.ToTable("PedidoItens");
                item.WithOwner().HasForeignKey("PedidoId");
                item.Property<int>("Id");
                item.HasKey("Id");
                item.Property(i => i.ProdutoId).IsRequired().HasMaxLength(50);
                item.Property(i => i.Quantidade).IsRequired();
                item.Property(i => i.PrecoUnitario).HasPrecision(18, 2).IsRequired();
            });
        });
    }
}
