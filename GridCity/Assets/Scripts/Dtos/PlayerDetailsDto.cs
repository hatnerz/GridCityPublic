using System;
using UnityEngine;

namespace Assets.Scripts.Dtos
{
    public class PlayerDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public int Score { get; set; }
    }
}
