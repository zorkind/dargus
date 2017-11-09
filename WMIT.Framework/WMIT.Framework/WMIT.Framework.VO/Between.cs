using System;

namespace WMIT.Framework.VO
{
    /// <summary>
    /// Passe os valores Min e Max iguais para consultar apenas 1 valor. Passando apenas o Min, será realizado um Maior que, se for passado apenas o Max será realizado um Menor que.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Between<T>
    {
        public Between()
        {
            Min = default(T);
            Max = default(T);
        }

        public Between(T Min, T Max)
        {
            this.Min = Min;
            this.Max = Max;
        }

        /// <summary>
        /// Passe os valores Min e Max iguais para consultar apenas 1 valor. Passando apenas o Min, será realizado um Maior que, se for passado apenas o Max será realizado um Menor que.
        /// </summary>
        public T Min { get; set; }

        /// <summary>
        /// Passe os valores Min e Max iguais para consultar apenas 1 valor. Passando apenas o Min, será realizado um Maior que, se for passado apenas o Max será realizado um Menor que.
        /// </summary>
        public T Max { get; set; }

        /// <summary>
        /// Determina se o framework deve utilizar Maior, ou Maior / Igual, e Menor, ou Menor / Igual.
        /// </summary>
        public bool UseEquals { get; set; }
    }
}
