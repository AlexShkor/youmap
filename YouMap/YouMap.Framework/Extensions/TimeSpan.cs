using System;
using System.Globalization;

namespace YouMap.Framework.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ToPastString(this TimeSpan source)
        {
            if (source.TotalSeconds < 60)
            {
                return String.Format("{0} секунд назад", source.TotalSeconds);
            }
            if (source.TotalMinutes < 60)
            {
                return String.Format("{0} минут назад", source.TotalMinutes);
            }
            if (source.TotalHours < 24)
            {
                return String.Format("{0} часов назад", source.TotalHours);
            }
            return String.Format("{0} дней назад назад", source.TotalDays);
        }

        public static string ToFutureString(this TimeSpan source)
        {
            if (source.TotalSeconds <= 5)
            {
                return String.Format("через 5 секунд");
            }
            if (source.TotalSeconds < 60)
            {
                return String.Format("через {0} секунд",source.TotalSeconds);
            }
            if (source.TotalMinutes < 2)
            {
                return String.Format("через минуту");
            }
            if (source.TotalMinutes < 3)
            {
                return String.Format("через две минуты");
            }
            if (source.TotalMinutes < 4)
            {
                return String.Format("через три минуты");
            }
            if (source.TotalMinutes < 60)
            {
                return String.Format("через {0} минут", source.TotalMinutes);
            }
            /*
             (- 5 сек) = 5 секунд назад
                (- 1 мин) = минуту назад
                (- 2 мин) = две минуты назад
                (- 3 мин) = три минуты назад
                (- 4 мин) = 4 минуты наза
                (- 1 час) = час назад
                (- 1 час 15 мин) = час назад
                (- 3 часа) = три часа назад
                (- 4 часа) = 4 часа назад
                (- 1 день) = вчера в {время}
                (- 2 дня) = 20 мар в 17:50

                (+ 5 сек) = через 5 сек
                (+ 1 мин) = через минуту
                (+ 1мин 5 сек) = через минуту
                (+ 3 мин) = через три минуты
                (+ 4 мин) = через 4 минуты
                (+ 1 час) = через час
                (+ 3 часа) = через три часа
                (+ 4 часа) = через 4 часа
                (+ 1 день) = завтра в {время}
                (+ 2 дня) = 2 апреля в {время}

             * 
             * 
             * 
             * 
             */
            return source.ToString();
        }

        public static string ToInfoString(this DateTime date)
        {
            var current = DateTime.Now;
            var timespan = current - date;
            if (Math.Abs(timespan.TotalMinutes) < 60)
            {
                if (timespan.TotalSeconds >= 0)
                {
                    return timespan.ToPastString();
                }
                return timespan.ToFutureString();
            }
            if (current.Date == date.Date)
            {
                return "сегодня в " + date.ToString("t",CultureInfo.GetCultureInfo("ru-RU"));
            }
            if (current.Date.AddDays(1) == date.Date)
            {
                return "завтра в " + date.ToString("t", CultureInfo.GetCultureInfo("ru-RU"));
            }
            return date.ToString("dd MMMM HH:mm", CultureInfo.GetCultureInfo("ru-RU"));
        }
    }
}