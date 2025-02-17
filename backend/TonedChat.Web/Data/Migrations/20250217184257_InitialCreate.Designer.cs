﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TonedChat.Web.Data;

#nullable disable

namespace TonedChat.Web.Data.Migrations
{
    [DbContext(typeof(TautDatabaseContext))]
    [Migration("20250217184257_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.2");

            modelBuilder.Entity("TonedChat.Web.Data.Entities.ChatMessageEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasColumnName("id");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("content");

                    b.Property<string>("CreateDate")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("create_date");

                    b.Property<string>("Date")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("date");

                    b.Property<string>("UpdateDate")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("update_date");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_chat_message");

                    b.ToTable("chat_message", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
