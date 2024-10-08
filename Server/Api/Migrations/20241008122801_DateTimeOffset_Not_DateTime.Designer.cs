﻿// <auto-generated />
using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ReaktlyC.Migrations
{
    [DbContext(typeof(Repository))]
    [Migration("20241008122801_DateTimeOffset_Not_DateTime")]
    partial class DateTimeOffset_Not_DateTime
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<TimeSpan>("PreparationDuration")
                        .HasColumnType("time");

                    b.Property<string>("RoomId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTimeOffset?>("StartClickedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CurrentMiniGameId");

                    b.HasIndex("RoomId");

                    b.ToTable("Games");
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

                    b.Property<DateTimeOffset>("DisplayTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Word")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ColorTapRoundId");

                    b.ToTable("ColorTapWordPairDisplay");
                });

            modelBuilder.Entity("Domain.MiniGames.MiniGame", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CurrentRoundId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("CurrentRoundNo")
                        .HasColumnType("int");

                    b.Property<string>("Discriminator")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GameId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Instructions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("InstructionsDuration")
                        .HasColumnType("time");

                    b.Property<DateTimeOffset?>("InstructionsStartTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("MiniGameType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("RoundDuration")
                        .HasColumnType("time");

                    b.Property<int>("TotalRoundsNo")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CurrentRoundId");

                    b.HasIndex("GameId");

                    b.ToTable("MiniGames");

                    b.HasDiscriminator<string>("MiniGameType").HasValue("MiniGame");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Domain.MiniGames.MiniGameRound", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTimeOffset>("EndTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("MiniGameId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoundType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("StartTime")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("MiniGameId");

                    b.ToTable("MiniGameRounds");

                    b.HasDiscriminator<string>("RoundType").HasValue("MiniGameRound");

                    b.UseTphMappingStrategy();
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

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CurrentGameId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("HostId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CurrentGameId");

                    b.ToTable("Rooms");
                });

            modelBuilder.Entity("Domain.MiniGames.ColorTapGame", b =>
                {
                    b.HasBaseType("Domain.MiniGames.MiniGame");

                    b.HasDiscriminator().HasValue("ColorTapGame");
                });

            modelBuilder.Entity("Domain.MiniGames.ColorTapRound", b =>
                {
                    b.HasBaseType("Domain.MiniGames.MiniGameRound");

                    b.HasDiscriminator().HasValue("ColorTapRound");
                });

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.HasOne("Domain.MiniGames.MiniGame", "CurrentMiniGame")
                        .WithMany()
                        .HasForeignKey("CurrentMiniGameId");

                    b.HasOne("Domain.Room", null)
                        .WithMany("PastGames")
                        .HasForeignKey("RoomId");

                    b.Navigation("CurrentMiniGame");
                });

            modelBuilder.Entity("Domain.MiniGames.ColorTapWordPairDisplay", b =>
                {
                    b.HasOne("Domain.MiniGames.ColorTapRound", null)
                        .WithMany("ColorWordPairs")
                        .HasForeignKey("ColorTapRoundId");
                });

            modelBuilder.Entity("Domain.MiniGames.MiniGame", b =>
                {
                    b.HasOne("Domain.MiniGames.MiniGameRound", "CurrentRound")
                        .WithMany()
                        .HasForeignKey("CurrentRoundId");

                    b.HasOne("Domain.Game", null)
                        .WithMany("MiniGames")
                        .HasForeignKey("GameId");

                    b.Navigation("CurrentRound");
                });

            modelBuilder.Entity("Domain.MiniGames.MiniGameRound", b =>
                {
                    b.HasOne("Domain.MiniGames.MiniGame", null)
                        .WithMany("Rounds")
                        .HasForeignKey("MiniGameId");
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
                        .WithMany("Scoreboard")
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
                    b.HasOne("Domain.Game", "CurrentGame")
                        .WithMany()
                        .HasForeignKey("CurrentGameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CurrentGame");
                });

            modelBuilder.Entity("Domain.Game", b =>
                {
                    b.Navigation("MiniGames");

                    b.Navigation("Scoreboard");
                });

            modelBuilder.Entity("Domain.MiniGames.MiniGame", b =>
                {
                    b.Navigation("Rounds");
                });

            modelBuilder.Entity("Domain.Room", b =>
                {
                    b.Navigation("PastGames");

                    b.Navigation("Players");
                });

            modelBuilder.Entity("Domain.MiniGames.ColorTapRound", b =>
                {
                    b.Navigation("ColorWordPairs");
                });
#pragma warning restore 612, 618
        }
    }
}
