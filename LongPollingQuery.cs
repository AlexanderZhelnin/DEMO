using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Demo;

#region LongPollingValue
/// <summary>
/// Значение очереди
/// </summary>
/// <typeparam name="K"></typeparam>
public class LongPollingValue<K>
{
    /// <summary>
    /// Текущие занчение
    /// </summary>
    public K Value { get; set; }
    /// <summary>
    /// Маркер по котором считываем новые значения
    /// </summary>
    public DateTime Marker { get; set; }

    /// <summary>
    /// Следующий элемент в связанном списке
    /// </summary>
    [JsonIgnore]
    public LongPollingValue<K> Next { get; set; }
}
#endregion

/// <summary>
/// Очередь реализующаю паттерн LongPolling ( "Длительный Опрос" )
/// </summary>
/// <typeparam name="T">generetic тип очереди</typeparam>
public class LongPollingQuery<T>
{
    #region Свойства
    private LongPollingValue<T> _first { get; set; }
    private LongPollingValue<T> _last { get; set; }

    /// <summary>
    /// Максимальное время удержания запроса, когда нет подходящих данных
    /// </summary>
    public TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Время устаревания данных в очереди
    /// </summary>
    public TimeSpan WatchDogTimeOut { get; set; } = TimeSpan.FromMinutes(5);
    #endregion

    #region События
    private event Action Added;
    #endregion

    #region Конструктор
    /// <summary>
    /// Конструктор
    /// </summary>
    public LongPollingQuery()
    {
        var clearwatchdog = Observable.FromEvent(h => Added += h, h => Added -= h);

        clearwatchdog
            .Sample(TimeSpan.FromMinutes(1))
            .Subscribe(_ =>
            {
                var dt = DateTime.Now - WatchDogTimeOut;
                while (_first != null && _first.Marker < dt) _first = _first.Next;
            });
    }
    #endregion

    private readonly object _lock = new();
    #region Add
    /// <summary>
    /// Добавление в очередь
    /// </summary>        
    public void Add(T value)
    {
        lock (_lock)
        {
            var added = new LongPollingValue<T> { Value = value, Marker = DateTime.Now };
            if (_first == null)
                _first = _last = added;
            else
                _last = _last.Next = added;
        }
        Added();
    }
    #endregion

    #region Read
    /// <summary>
    /// Чтение из очереди
    /// </summary>
    /// <param name="marker">маркер после которого происходит чтение данных из очереди</param>        
    /// <returns></returns>
    public async IAsyncEnumerable<LongPollingValue<T>> Read(DateTime marker)
    {
        await Task.Delay(500);
        var dtstart = DateTime.Now;
        var fined = false;
        do
        {
            var item = _first;
            while (item != null)
            {
                fined = item.Marker > marker;
                if (fined) yield return item;

                item = item.Next;
            }
            if (!fined) await Task.Delay(500);
        }
        while (!fined && (DateTime.Now - dtstart) < TimeOut);
    }
    #endregion
}
