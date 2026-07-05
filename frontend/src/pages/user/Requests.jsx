export default function Requests() {
  return (
    <div>
      <h2>Электронный запрос</h2>

      <label>Тип запроса</label>
      <select>
        <option>Справка</option>
        <option>Уведомление</option>
      </select>

      <br />

      <textarea placeholder="Комментарии..." />

      <br />

      <button>Запросить</button>
      <button>Отправить уведомление</button>
    </div>
  )
}