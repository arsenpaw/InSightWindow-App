﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations; 
using System.Reflection.Metadata;


namespace AXProductApp.Models.DeviceModel
{
    public class Device
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get;  set; } = Guid.NewGuid();

        public required string DeviceType { get; set; } 

        public virtual Guid? UserId { get; set; }


    }
}