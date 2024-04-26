﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YYBagProgram.Models
{
    [DisplayName("會員")]
    public class Members
    {
        [DisplayName("會員帳號")]
        [Required]
        [Column(TypeName = "varchar(10)")]
        public string strMemberId { get; set; } = string.Empty;

        [DisplayName("密碼")]
        [Required]
        [Column(TypeName = "varchar(max)")]
        public string? strMemberPassWord { get; set; } = string.Empty;

        [DisplayName("姓名")]
        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public string strMemberName { get; set; } = string.Empty;

        [DisplayName("手機")]
        [Column(TypeName = "varchar(15)")]  
        public string? strMemberPhone { get; set; } = string.Empty;

        [DisplayName("Email")]
        [Column(TypeName = "varchar(30)")]
        [Required]
        public string strMemberEmail { get; set; } = string.Empty;

        [DisplayName("生日")]
        [Column(TypeName = "date")]
        public DateTime? dateBirthday { get; set; }

    }
}
