// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Subless.Data;

namespace Subless.Data.Migrations
{
    [DbContext(typeof(Repository))]
    [Migration("20211114181122_CreatorWebhook")]
    partial class CreatorWebhook
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.11")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Subless.Models.Creator", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ActivationCode")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ActivationExpiration")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<Guid>("PartnerId")
                        .HasColumnType("uuid");

                    b.Property<string>("PayPalId")
                        .HasColumnType("text");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ActivationCode");

                    b.HasIndex("PartnerId");

                    b.HasIndex("UserId");

                    b.HasIndex("Username", "PartnerId")
                        .IsUnique();

                    b.ToTable("Creators");
                });

            modelBuilder.Entity("Subless.Models.Hit", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CognitoId")
                        .HasColumnType("text");

                    b.Property<Guid>("CreatorId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PartnerId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Uri")
                        .HasColumnType("text");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CognitoId");

                    b.HasIndex("TimeStamp");

                    b.HasIndex("UserId");

                    b.ToTable("Hits");
                });

            modelBuilder.Entity("Subless.Models.Partner", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("Admin")
                        .HasColumnType("uuid");

                    b.Property<string>("CognitoAppClientId")
                        .HasColumnType("text");

                    b.Property<string>("CreatorActivatedWebhook")
                        .HasColumnType("text");

                    b.Property<string>("PayPalId")
                        .HasColumnType("text");

                    b.Property<string>("Site")
                        .HasColumnType("text");

                    b.Property<string>("UserPattern")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Admin");

                    b.HasIndex("CognitoAppClientId")
                        .IsUnique();

                    b.ToTable("Partners");
                });

            modelBuilder.Entity("Subless.Models.Payee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("PayPalId")
                        .HasColumnType("text");

                    b.Property<double>("Payment")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("Payee");
                });

            modelBuilder.Entity("Subless.Models.Payer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("Payment")
                        .HasColumnType("double precision");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Payer");
                });

            modelBuilder.Entity("Subless.Models.Payment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("Amount")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("DateSent")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid?>("PayeeId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("PayerId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("PayeeId");

                    b.HasIndex("PayerId");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("Subless.Models.PaymentAuditLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("DatePaid")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("PayPalId")
                        .HasColumnType("text");

                    b.Property<double>("Payment")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("PaymentAuditLogs");
                });

            modelBuilder.Entity("Subless.Models.RuntimeConfiguration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AdminKey")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("Subless.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CognitoId")
                        .HasColumnType("text");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<string>("StripeCustomerId")
                        .HasColumnType("text");

                    b.Property<string>("StripeSessionId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CognitoId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Subless.Models.Creator", b =>
                {
                    b.HasOne("Subless.Models.Partner", null)
                        .WithMany("Creators")
                        .HasForeignKey("PartnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Subless.Models.User", null)
                        .WithMany("Creators")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Subless.Models.Hit", b =>
                {
                    b.HasOne("Subless.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Subless.Models.Partner", b =>
                {
                    b.HasOne("Subless.Models.User", null)
                        .WithMany("Partners")
                        .HasForeignKey("Admin")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Subless.Models.Payment", b =>
                {
                    b.HasOne("Subless.Models.Payee", "Payee")
                        .WithMany()
                        .HasForeignKey("PayeeId");

                    b.HasOne("Subless.Models.Payer", "Payer")
                        .WithMany()
                        .HasForeignKey("PayerId");

                    b.Navigation("Payee");

                    b.Navigation("Payer");
                });

            modelBuilder.Entity("Subless.Models.Partner", b =>
                {
                    b.Navigation("Creators");
                });

            modelBuilder.Entity("Subless.Models.User", b =>
                {
                    b.Navigation("Creators");

                    b.Navigation("Partners");
                });
#pragma warning restore 612, 618
        }
    }
}
