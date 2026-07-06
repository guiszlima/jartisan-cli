using System;
using System.Text.RegularExpressions; // 1. ADICIONADO: Necessário para o Regex e GeneratedRegex

namespace Jartisan.Application.Utils // Corrigido o erro de digitação de "Utills" para "Utils"
{
    public static partial class KebabRegex // 2. CORREÇÃO: Nome da classe alterado
    {
        // O Source Generator vai criar a implementação desse método em tempo de build (AOT Safe)
        [GeneratedRegex("(?<!^)(?=[A-Z])")]
        private static partial Regex GetKebabRegex(); // 3. CORREÇÃO: Nome do método alterado para não conflitar

        // Método público para você chamar em outros lugares do código
        public static string Convert(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            
            // Usa a Regex compilada para injetar o hífen e joga tudo para minúsculo
            return GetKebabRegex().Replace(input, "-").ToLower();
        }
    }
}
