﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Subless.Data;

#nullable disable

namespace Subless.Data.Migrations
{
    [DbContext(typeof(Repository))]
    [Migration("20220815185149_AddedReportingValuesForStripe")]
    partial class AddedReportingValuesForStripe
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Subless.Models.CalculatorExecution", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("DateExecuted")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("DateQueued")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsProcessing")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("PeriodEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("PeriodStart")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Result")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("CalculatorExecutions");
                });

            modelBuilder.Entity("Subless.Models.Creator", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("AcceptedTerms")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("ActivationCode")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("ActivationExpiration")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("CreateDate")
                        .HasColumnType("timestamp with time zone");

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

                    b.Property<DateTimeOffset>("TimeStamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Uri")
                        .HasColumnType("text");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("CognitoId");

                    b.HasIndex("TimeStamp");

                    b.HasIndex("UserId");

                    b.HasIndex("TimeStamp", "CognitoId");

                    b.HasIndex("TimeStamp", "CreatorId");

                    b.HasIndex("TimeStamp", "PartnerId");

                    b.ToTable("Hits");
                });

            modelBuilder.Entity("Subless.Models.IdleEmailExecution", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("DateExecuted")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("DateQueued")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsProcessing")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("PeriodEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("PeriodStart")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("IdleEmailExecutions");
                });

            modelBuilder.Entity("Subless.Models.Partner", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("AcceptedTerms")
                        .HasColumnType("boolean");

                    b.Property<Guid>("Admin")
                        .HasColumnType("uuid");

                    b.Property<string>("CognitoAppClientId")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatorWebhook")
                        .HasColumnType("text");

                    b.Property<string>("PayPalId")
                        .HasColumnType("text");

                    b.Property<string[]>("Sites")
                        .HasColumnType("text[]");

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

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("PayPalId")
                        .HasColumnType("text");

                    b.Property<int>("PayeeType")
                        .HasColumnType("integer");

                    b.Property<double>("Payment")
                        .HasColumnType("double precision");

                    b.Property<Guid>("TargetId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Payee");
                });

            modelBuilder.Entity("Subless.Models.Payer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("Fees")
                        .HasColumnType("double precision");

                    b.Property<double>("Payment")
                        .HasColumnType("double precision");

                    b.Property<double>("Taxes")
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

                    b.Property<DateTimeOffset>("DateSent")
                        .HasColumnType("timestamp with time zone");

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

                    b.Property<DateTimeOffset>("DatePaid")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("Fees")
                        .HasColumnType("double precision");

                    b.Property<string>("PayPalId")
                        .HasColumnType("text");

                    b.Property<int>("PayeeType")
                        .HasColumnType("integer");

                    b.Property<double>("Payment")
                        .HasColumnType("double precision");

                    b.Property<DateTimeOffset>("PaymentPeriodEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("PaymentPeriodStart")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("Revenue")
                        .HasColumnType("double precision");

                    b.Property<Guid>("TargetId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("PaymentAuditLogs");
                });

            modelBuilder.Entity("Subless.Models.PaymentExecution", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("DateExecuted")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("DateQueued")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsCompleted")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsProcessing")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("PeriodEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("PeriodStart")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("PaymentExecutions");
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

            modelBuilder.Entity("Subless.Models.SublessUserSession", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("ApplicationName")
                        .HasColumnType("text");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("Expires")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Renewed")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SessionId")
                        .HasColumnType("text");

                    b.Property<string>("SubjectId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Ticket")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UserSessions");
                });

            modelBuilder.Entity("Subless.Models.UsageStat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UsageType")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("UsageStats");
                });

            modelBuilder.Entity("Subless.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<bool>("AcceptedTerms")
                        .HasColumnType("boolean");

                    b.Property<string>("CognitoId")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("CreateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<bool>("Replica_IsPaying")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("Replica_SubcriptionDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long?>("Replica_Subscription")
                        .HasColumnType("bigint");

                    b.Property<string>("StripeCustomerId")
                        .HasColumnType("text");

                    b.Property<string>("StripeSessionId")
                        .HasColumnType("text");

                    b.Property<bool>("WelcomeEmailSent")
                        .HasColumnType("boolean");

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
