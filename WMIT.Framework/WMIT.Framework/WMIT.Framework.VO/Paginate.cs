using System;
using System.Configuration;

namespace WMIT.Framework.VO
{
    public class Paginate
    {
        public Paginate()
        {
            HandleDefaults();
        }

        private static int BotoesPorTelaDefault;
        private static int LinhasPorPaginaDefault;

        private void HandleDefaults()
        {
            int lTry = default(int);

            if(Paginate.BotoesPorTelaDefault == default(int))
            {
                var lBotoesPorTelaDefault = ConfigurationManager.AppSettings["Paginate.BotoesPorTela"];
                if (lBotoesPorTelaDefault == null)
                    throw new NullReferenceException(Properties.Mensagens.PaginateBotoesPorTela);

                if (int.TryParse(lBotoesPorTelaDefault, out lTry) && lTry > 0)
                    BotoesPorTelaDefault = lTry;
                else
                    throw new InvalidCastException(Properties.Mensagens.PaginateBotoesPorTela);
            }

            if (Paginate.LinhasPorPaginaDefault == default(int))
            {
                var lLinhasPorPagina = ConfigurationManager.AppSettings["Paginate.LinhasPorPagina"];
                if (lLinhasPorPagina == null)
                    throw new NullReferenceException(Properties.Mensagens.PaginateLinhasPorPagina);

                lTry = default(int);
                if (int.TryParse(lLinhasPorPagina, out lTry) && lTry > 0)
                    LinhasPorPaginaDefault = lTry;
                else
                    throw new InvalidCastException(Properties.Mensagens.PaginateLinhasPorPagina);
            }

            LinhasPorPagina = Paginate.LinhasPorPaginaDefault;
            BotoesPorTela = Paginate.BotoesPorTelaDefault;
        }

        public bool Enabled { get; set; }
        public int LinhasPorPagina { get; set; }
        public int Pagina { get; set; }
        public int BotoesPorTela { get; set; }
        public int StartingPage
        {
            get
            {
                if (BotoesPorTela > default(int))
                {
                    var res = Pagina / BotoesPorTela;
                    if (Pagina == BotoesPorTela)
                        return 1;
                    else
                    {
                        if (res > 0)
                        {
                            var mu = (res * BotoesPorTela);
                            if (mu == Pagina)
                                return mu - (BotoesPorTela - 1);
                            else
                                return ++mu;
                        }
                        else
                            return 1;
                    }
                }
                else
                    return 1;
            }
        }

        public long TotalLinhas { get; set; }
        public long TotalPaginas
        {
            get
            {
                return Convert.ToInt64(Math.Ceiling((double)TotalLinhas / LinhasPorPagina));
            }
        }
    }
}
