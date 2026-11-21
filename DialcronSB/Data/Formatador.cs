using System.Text;

namespace DialcronSB.Data;

public static class Formatador
{
    public static string SomenteDigitos(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return string.Empty;

        var builder = new StringBuilder(valor.Length);
        foreach (var caractere in valor)
            if (char.IsDigit(caractere))
                builder.Append(caractere);

        return builder.ToString();
    }

    // Exibição: (11) 91234-5678 / (11) 1234-5678 / (11) / ""
    public static string Telefone(int? ddd, int? numero)
    {
        var numeroFormatado = NumeroTelefone(numero);

        if (ddd is null)
            return numeroFormatado;

        return string.IsNullOrEmpty(numeroFormatado)
            ? $"({ddd})"
            : $"({ddd}) {numeroFormatado}";
    }

    // Exibição do número sozinho (DDD está em outro campo)
    public static string NumeroTelefone(int? numero)
    {
        if (numero is null)
            return string.Empty;

        var texto = numero.Value.ToString();

        return texto.Length switch
        {
            8 => $"{texto[..4]}-{texto[4..]}",
            9 => $"{texto[..5]}-{texto[5..]}",
            _ => texto
        };
    }

    // Exibição: CPF 000.000.000-00 | CNPJ 00.000.000/0000-00
    public static string CpfCnpj(string? documento)
    {
        var digitos = SomenteDigitos(documento);

        return digitos.Length switch
        {
            11 => $"{digitos[..3]}.{digitos[3..6]}.{digitos[6..9]}-{digitos[9..]}",
            14 => $"{digitos[..2]}.{digitos[2..5]}.{digitos[5..8]}/{digitos[8..12]}-{digitos[12..]}",
            _ => documento ?? string.Empty
        };
    }

    // Exibição: 00000-000 (completa com zero à esquerda se necessário)
    public static string Cep(int? cep)
    {
        if (cep is null)
            return string.Empty;

        var texto = cep.Value.ToString("D8");
        return $"{texto[..5]}-{texto[5..]}";
    }

    // ---- Máscaras para digitação (Entry) ----

    public static string MascaraDdd(string? texto)
    {
        var digitos = SomenteDigitos(texto);
        return digitos.Length > 2 ? digitos[..2] : digitos;
    }

    public static string MascaraNumeroTelefone(string? texto)
    {
        var digitos = SomenteDigitos(texto);

        if (digitos.Length > 9)
            digitos = digitos[..9];

        return digitos.Length switch
        {
            <= 4 => digitos,
            <= 8 => $"{digitos[..4]}-{digitos[4..]}",
            _ => $"{digitos[..5]}-{digitos[5..]}"
        };
    }

    public static string MascaraCpfCnpj(string? texto)
    {
        var d = SomenteDigitos(texto);

        if (d.Length > 14)
            d = d[..14];

        return d.Length switch
        {
            <= 3 => d,
            <= 6 => $"{d[..3]}.{d[3..]}",
            <= 9 => $"{d[..3]}.{d[3..6]}.{d[6..]}",
            <= 11 => $"{d[..3]}.{d[3..6]}.{d[6..9]}-{d[9..]}",
            <= 12 => $"{d[..2]}.{d[2..5]}.{d[5..8]}/{d[8..]}",
            _ => $"{d[..2]}.{d[2..5]}.{d[5..8]}/{d[8..12]}-{d[12..]}"
        };
    }

    public static string MascaraCep(string? texto)
    {
        var d = SomenteDigitos(texto);

        if (d.Length > 8)
            d = d[..8];

        return d.Length <= 5 ? d : $"{d[..5]}-{d[5..]}";
    }
}