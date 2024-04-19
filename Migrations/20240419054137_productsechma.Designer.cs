﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using YYBagProgram.Data;

#nullable disable

namespace YYBagProgram.Migrations
{
    [DbContext(typeof(YYBagProgramContext))]
    [Migration("20240419054137_productsechma")]
    partial class productsechma
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.29")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("YYBagProgram.Models.Classification", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(10)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Classification");
                });

            modelBuilder.Entity("YYBagProgram.Models.ClassificationDetail", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(10)");

                    b.Property<string>("strBagsId")
                        .HasColumnType("varchar(8)");

                    b.HasKey("Id", "strBagsId");

                    b.ToTable("ClassificationDetail");
                });

            modelBuilder.Entity("YYBagProgram.Models.Members", b =>
                {
                    b.Property<string>("strMemberPhone")
                        .HasColumnType("varchar(15)");

                    b.Property<DateTime>("dateBirthday")
                        .HasColumnType("date");

                    b.Property<string>("strMemberEmail")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<string>("strMemberId")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

                    b.Property<string>("strMemberName")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("strMemberPassWord")
                        .IsRequired()
                        .HasColumnType("varchar(max)");

                    b.HasKey("strMemberPhone");

                    b.ToTable("Member");
                });

            modelBuilder.Entity("YYBagProgram.Models.MonthlyHot", b =>
                {
                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.Property<int>("Month")
                        .HasColumnType("int");

                    b.Property<string>("strBagsId")
                        .HasColumnType("varchar(8)");

                    b.Property<string>("img")
                        .HasColumnType("varchar(max)");

                    b.HasKey("Year", "Month", "strBagsId");

                    b.ToTable("MonthlyHots");
                });

            modelBuilder.Entity("YYBagProgram.Models.Order", b =>
                {
                    b.Property<string>("strOrderGuid")
                        .HasColumnType("varchar(20)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("date");

                    b.Property<int>("iMemberId")
                        .HasColumnType("int");

                    b.Property<string>("strReceiver")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("strReceiverAddress")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("strReceiverPhone")
                        .IsRequired()
                        .HasColumnType("varchar(15)");

                    b.HasKey("strOrderGuid");

                    b.ToTable("Order");
                });

            modelBuilder.Entity("YYBagProgram.Models.OrderDetail", b =>
                {
                    b.Property<string>("strOrderGuid")
                        .HasColumnType("varchar(20)");

                    b.Property<int>("iFinalPrice")
                        .HasColumnType("int");

                    b.Property<int>("iOrderStatus")
                        .HasColumnType("int");

                    b.Property<int>("iQuantity")
                        .HasColumnType("int");

                    b.Property<string>("strBagsId")
                        .IsRequired()
                        .HasColumnType("varchar(8)");

                    b.HasKey("strOrderGuid");

                    b.ToTable("OrderDetail");
                });

            modelBuilder.Entity("YYBagProgram.Models.Product", b =>
                {
                    b.Property<string>("strBagsId")
                        .HasColumnType("varchar(8)");

                    b.Property<int>("BagType")
                        .HasColumnType("int");

                    b.Property<decimal>("dHigh")
                        .HasColumnType("decimal(6,2)");

                    b.Property<decimal>("dLength")
                        .HasColumnType("decimal(6,2)");

                    b.Property<decimal>("dWeight")
                        .HasColumnType("decimal(6,2)");

                    b.Property<decimal>("dWidth")
                        .HasColumnType("decimal(6,2)");

                    b.Property<int>("iPrice")
                        .HasColumnType("int");

                    b.Property<string>("strBagsName")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("strDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("strImageUrl")
                        .IsRequired()
                        .HasColumnType("varchar(max)");

                    b.Property<string>("strMaterial")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("strBagsId");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("YYBagProgram.Models.ProductColor", b =>
                {
                    b.Property<string>("strID")
                        .HasColumnType("varchar(20)");

                    b.Property<string>("strBagsId")
                        .IsRequired()
                        .HasColumnType("varchar(8)");

                    b.Property<string>("strColor")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("strID");

                    b.ToTable("ProductColor");
                });

            modelBuilder.Entity("YYBagProgram.Models.ProductsColorDetail", b =>
                {
                    b.Property<string>("strID")
                        .HasColumnType("varchar(20)");

                    b.Property<string>("strColor")
                        .HasColumnType("nvarchar(10)");

                    b.Property<int>("ProductStatus")
                        .HasColumnType("int");

                    b.Property<string>("Images")
                        .HasColumnType("varchar(max)");

                    b.Property<int>("iDeliveryDays")
                        .HasColumnType("int");

                    b.Property<int>("iPrice")
                        .HasColumnType("int");

                    b.Property<int>("iRemain")
                        .HasColumnType("int");

                    b.Property<int>("iTotal")
                        .HasColumnType("int");

                    b.Property<bool>("isOnline")
                        .HasColumnType("boolean ");

                    b.HasKey("strID", "strColor", "ProductStatus");

                    b.ToTable("ProductsColorDetail");
                });
#pragma warning restore 612, 618
        }
    }
}
