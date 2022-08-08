using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorldBank.Entities.DataModel;

namespace WorldBank.Entities
{
    public class WorldBankDBContext:DbContext
    {
        public WorldBankDBContext()
        {
            
        }
        public WorldBankDBContext(DbContextOptions<WorldBankDBContext> options)
          : base(options)
        {
            //Database.EnsureCreated();
        }

        public virtual DbSet<AuditTypes> AuditTypes { get; set; }
        public virtual DbSet<BankAccount> BankAccount { get; set; }
        public virtual DbSet<BankAccountLedger> BankAccountLedger { get; set; }
        public virtual DbSet<BankAccountTypes> BankAccountTypes { get; set; }
        public virtual DbSet<Currency> Currency { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<StaffAuditLog> StaffAuditLog { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<TransactionTypes> TransactionTypes { get; set; }
        public virtual DbSet<TransactionCharges> TransactionCharges { get; set; }
        public virtual DbSet<GeneratedTransactionNumber> GeneratedTransactionNumbers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GeneratedTransactionNumber>(entity =>
            {
                entity.HasKey(e => e.TransactionNo)
                   .HasName("PK_GeneratedTransactionNumber");

                entity.ToTable("generated_transaction_number");

                entity.Property(e => e.TransactionNo).HasColumnName("transaction_no");

                entity.Property(e => e.GeneratedNo)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("generated_no");
            });

            modelBuilder.Entity<TransactionCharges>(entity =>
            {
                entity.ToTable("transaction_charges");

                entity.Property(e => e.TransactionChargesId)
                    .ValueGeneratedNever()
                    .HasColumnName("transaction_charges_id");

                entity.Property(e => e.ChargesType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("charges_type");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("description");

                entity.Property(e => e.Percentage)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("percentage");

                entity.Property(e => e.UpdatedOn)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_on");
            });
            modelBuilder.Entity<AuditTypes>(entity =>
            {
                entity.HasKey(e => e.AuditTypeId)
                    .HasName("PK__audit_ty__8F8EB2E1A4C3B127");

                entity.ToTable("audit_types");

                entity.HasIndex(e => e.AuditType, "audit_types_index_5")
                    .IsUnique();

                entity.Property(e => e.AuditTypeId)
                    .ValueGeneratedNever()
                    .HasColumnName("audit_type_id");

                entity.Property(e => e.AuditType)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("audit_type");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<BankAccount>(entity =>
            {
                entity.ToTable("bank_account");

                entity.HasIndex(e => e.IbanNumber, "bank_account_index_3")
                    .IsUnique();

                entity.Property(e => e.BankAccountId)
                    .ValueGeneratedNever()
                    .HasColumnName("bank_account_id");

                entity.Property(e => e.BankAccountTypeId).HasColumnName("bank_account_type_id");

                entity.Property(e => e.ClosingBalance)
                    .HasColumnType("money")
                    .HasColumnName("closing_balance");

                entity.Property(e => e.CurrencyId).HasColumnName("currency_id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.IbanNumber)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("iban_number");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TotalCredit)
                    .HasColumnType("money")
                    .HasColumnName("total_credit");

                entity.Property(e => e.TotalDebit)
                    .HasColumnType("money")
                    .HasColumnName("total_debit");

                entity.Property(e => e.UpdatedOn)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_on");

                entity.HasOne(d => d.BankAccountType)
                    .WithMany(p => p.BankAccount)
                    .HasForeignKey(d => d.BankAccountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_bank_account_bank_account_types");

               

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.BankAccount)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__bank_acco__custo__34C8D9D1");
            });

            modelBuilder.Entity<BankAccountLedger>(entity =>
            {
                entity.HasKey(e => e.LedgerId)
                    .HasName("PK__bank_acc__97EDEDA1527394AA");

                entity.ToTable("bank_account_ledger");

                entity.Property(e => e.LedgerId)
                    .ValueGeneratedNever()
                    .HasColumnName("ledger_id");

                entity.Property(e => e.BankAccountId).HasColumnName("bank_account_id");

                entity.Property(e => e.Credit)
                    .HasColumnType("money")
                    .HasColumnName("credit");

                entity.Property(e => e.Debit)
                    .HasColumnType("money")
                    .HasColumnName("debit");

                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

                entity.Property(e => e.TransactionTypeId).HasColumnName("transaction_type_id");

               
            });

            modelBuilder.Entity<BankAccountTypes>(entity =>
            {
                entity.HasKey(e => e.BankAccountTypeId)
                    .HasName("PK__bank_acc__472D068FCD4F2D9A");

                entity.ToTable("bank_account_types");

                entity.HasIndex(e => e.BankAccountType, "bank_account_types_index_2")
                    .IsUnique();

                entity.Property(e => e.BankAccountTypeId)
                    .ValueGeneratedNever()
                    .HasColumnName("bank_account_type_id");

                entity.Property(e => e.BankAccountType)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("bank_account_type");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<Currency>(entity =>
            {
                entity.ToTable("currency");

                entity.Property(e => e.CurrencyId)
                    .ValueGeneratedNever()
                    .HasColumnName("currency_id");

                entity.Property(e => e.CurrencyCode)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("currency_code");

                entity.Property(e => e.CurrencySymbol)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("currency_symbol");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("description");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customer");

                entity.Property(e => e.CustomerId)
                    .ValueGeneratedNever()
                    .HasColumnName("customer_id");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasColumnName("created_on");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("email");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("full_name");

                entity.Property(e => e.IdentityCardNo)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("identity_card_no");

                entity.Property(e => e.Mobile)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("mobile");

                entity.Property(e => e.MobileCode)
                    .IsRequired()
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasColumnName("mobile_code");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.SaltPassword)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("salt_password");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdatedOn)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.ToTable("staff");

                entity.Property(e => e.StaffId)
                    .ValueGeneratedNever()
                    .HasColumnName("staff_id");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasColumnName("created_on");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("full_name");

                entity.Property(e => e.LoginId)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("login_id");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("password");

                entity.Property(e => e.SaltPassword)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("salt_password");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdatedOn)
                    .HasColumnType("datetime")
                    .HasColumnName("updated_on");
            });

            modelBuilder.Entity<StaffAuditLog>(entity =>
            {
                entity.HasKey(e => e.StaffAuditId)
                    .HasName("PK__staff_au__D1CDE40E4FEE5CAF");

                entity.ToTable("staff_audit_log");

                entity.Property(e => e.StaffAuditId)
                    .ValueGeneratedNever()
                    .HasColumnName("staff_audit_id");

                entity.Property(e => e.AuditTypeId).HasColumnName("audit_type_id");
                entity.Property(e => e.CreatedOn).HasColumnName("created_on");

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasMaxLength(1000)
                    .HasColumnName("note");

                entity.Property(e => e.RecordId).HasColumnName("record_id");

                entity.Property(e => e.StaffId).HasColumnName("staff_id");

                entity.HasOne(d => d.AuditType)
                    .WithMany(p => p.StaffAuditLog)
                    .HasForeignKey(d => d.AuditTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_staff_audit_log_audit_types");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.StaffAuditLog)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__staff_aud__staff__3D5E1FD2");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.ToTable("transaction");

                entity.Property(e => e.TransactionId)
                    .ValueGeneratedNever()
                    .HasColumnName("transaction_id");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasColumnName("amount");

                entity.Property(e => e.BankAccountId).HasColumnName("bank_account_id");

                entity.Property(e => e.Charges)
                    .HasColumnType("money")
                    .HasColumnName("charges");

                entity.Property(e => e.ChargesPercentage)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("charges_percentage");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasColumnName("created_on");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.NetAmount)
                    .HasColumnType("money")
                    .HasColumnName("net_amount");

                entity.Property(e => e.Notes)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnName("notes");

                entity.Property(e => e.ReceiverBankAccountId).HasColumnName("receiver_bank_account_id");

                entity.Property(e => e.ReceiverCustomerId).HasColumnName("receiver_customer_id");

                entity.Property(e => e.TransactionNo)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("transaction_no");

                entity.Property(e => e.TransactionTypeId).HasColumnName("transaction_type_id");

                entity.HasOne(d => d.BankAccount)
                    .WithMany(p => p.Transaction)
                    .HasForeignKey(d => d.BankAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_transaction_bank_account_id");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Transaction)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_transaction_customer_id");

               
            });

            modelBuilder.Entity<TransactionTypes>(entity =>
            {
                entity.HasKey(e => e.TransactionTypeId)
                    .HasName("PK__transact__439ABFC1812B89B3");

                entity.ToTable("transaction_types");

                entity.HasIndex(e => e.TransactionType, "transaction_types_index_4")
                    .IsUnique();

                entity.Property(e => e.TransactionTypeId)
                    .ValueGeneratedNever()
                    .HasColumnName("transaction_type_id");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("description");

                entity.Property(e => e.TransactionType)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("transaction_type");
            });

        }

    }

    

  
}
