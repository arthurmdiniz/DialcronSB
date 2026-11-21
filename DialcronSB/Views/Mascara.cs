using System;
using System.Collections.Generic;
using System.Text;

namespace DialcronSB.Views;

public static class Mascara
{
    public static void Aplicar(Entry entry, Func<string?, string> formatar)
    {
        entry.Dispatcher.Dispatch(() =>
        {
            var atual = entry.Text;
            var formatado = formatar(atual);

            if (string.Equals(atual, formatado, StringComparison.Ordinal))
                return;

            entry.Text = formatado;
        });
    }
}
