using System;
using System.Collections.Generic;
using System.Text;

namespace DialcronSB.Views;

public static class Mascara
{
    public static void Aplicar(Entry entry, Func<string?, string> formatar)
    {
        var formatado = formatar(entry.Text);

        if (entry.Text == formatado)
            return;

        entry.Text = formatado;
        entry.CursorPosition = formatado.Length;
    }
}