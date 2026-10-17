using System.Globalization;

namespace DialcronSB.Data;

public static class Conversor
{
	public static int? ParaInt(string? texto)
	{
		return int.TryParse(texto?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var valor)
			? valor
			: null;
	}

	public static double ParaDouble(string? texto)
	{
		var normalizado = (texto ?? string.Empty).Trim().Replace("R$", string.Empty).Trim();

		if (normalizado.Contains(',') && normalizado.Contains('.'))
			normalizado = normalizado.Replace(".", string.Empty);

		normalizado = normalizado.Replace(',', '.');

		return double.TryParse(normalizado, NumberStyles.Float, CultureInfo.InvariantCulture, out var valor)
			? valor
			: 0;
	}

	public static string? ParaTexto(int? valor)
	{
		return valor?.ToString(CultureInfo.InvariantCulture);
	}

	public static string? ParaTextoOuNulo(string? texto)
	{
		return string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
	}

	public static string ParaSimNao(bool valor)
	{
		return valor ? "S" : "N";
	}
}
