﻿// <auto-generated />
using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ReaktlyC.Migrations
{
    [DbContext(typeof(Repository))]
    partial class RepositoryModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CurrentMiniGameId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<TimeSpan>("PreparationTime")
                        .HasColumnType("time");

                    b.Property<string>("RoomId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CurrentMiniGameId");

                    b.HasIndex("RoomId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Domain.MiniGame", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("CurrentRound")
                        .HasColumnType("int");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GameId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("InstructionStartTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Instructions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("InstructionsDuration")
                        .HasColumnType("time");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoundCount")
                        .HasColumnType("int");

                    b.Property<TimeSpan>("RoundDuration")
                        .HasColumnType("time");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("MiniGames");

                    b.HasDiscriminator<string>("Discriminator").HasValue("MiniGame");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Domain.MiniGames.ColorTapRound", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ColorTapGameId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ColorTapGameId");

                    b.ToTable("ColorTapRound");
                });

            modelBuilder.Entity("Domain.MiniGames.ColorTapWordPairDisplay", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ColorTapRoundId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DisplayTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Word")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ColorTapRoundId");

                    b.ToTable("ColorTapWordPairDisplay");
                });

            modelBuilder.Entity("Domain.Player", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoomId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoomId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Domain.PlayerScore", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("GameId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PlayerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Score")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("PlayerId");

                    b.ToTable("PlayerScores");
                });

            modelBuilder.Entity("Domain.Room", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("nvarchar(6)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("GameId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("HostId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("Domain.MiniGames.ColorTapGame", b =>
                {
                    b.HasBaseType("Domain.MiniGame");

                    b.HasDiscriminator().HasValue("ColorTapGame");
                });

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.HasOne("Domain.MiniGame", "CurrentMiniGame")
                        .WithMany()
                        .HasForeignKey("CurrentMiniGameId");

                    b.HasOne("Domain.Room", null)
                        .WithMany("PastGames")
                        .HasForeignKey("RoomId");

                    b.Navigation("CurrentMiniGame");
                });

            modelBuilder.Entity("Domain.MiniGame", b =>
                {
                    b.HasOne("Domain.Game", null)
                        .WithMany("MiniGames")
                        .HasForeignKey("GameId");
                });

            modelBuilder.Entity("Domain.MiniGames.ColorTapRound", b =>
                {
                    b.HasOne("Domain.MiniGames.ColorTapGame", null)
                        .WithMany("Rounds")
                        .HasForeignKey("ColorTapGameId");
                });

            modelBuilder.Entity("Domain.MiniGames.ColorTapWordPairDisplay", b =>
                {
                    b.HasOne("Domain.MiniGames.ColorTapRound", null)
                        .WithMany("ColorWordPairs")
                        .HasForeignKey("ColorTapRoundId");
                });

            modelBuilder.Entity("Domain.Player", b =>
                {
                    b.HasOne("Domain.Room", null)
                        .WithMany("Players")
                        .HasForeignKey("RoomId");
                });

            modelBuilder.Entity("Domain.PlayerScore", b =>
                {
                    b.HasOne("Domain.Game", "Game")
                        .WithMany("PlayerScores")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("Player");
                });

            modelBuilder.Entity("Domain.Room", b =>
                {
                    b.HasOne("Domain.Game", "Game")
                        .WithMany()
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.Navigation("MiniGames");

                    b.Navigation("PlayerScores");
                });

            modelBuilder.Entity("Domain.MiniGames.ColorTapRound", b =>
                {
                    b.Navigation("ColorWordPairs");
                });

            modelBuilder.Entity("Domain.Room", b =>
                {
                    b.Navigation("PastGames");

                    b.Navigation("Players");
                });

            modelBuilder.Entity("Domain.MiniGames.ColorTapGame", b =>
                {
                    b.Navigation("Rounds");
                });
#pragma warning restore 612, 618
        }
    }
}
