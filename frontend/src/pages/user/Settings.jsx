export default function Settings() {
  return (
    <div>
      <h2>Настройка пользователя</h2>

      <label>Телефон</label>
      <input value="+375 (29) 123-45-67" readOnly />

      <label>Email</label>
      <input value="user@mail.com" readOnly />

      <h4>Уведомления</h4>

      <label><input type="checkbox" /> SMS</label>
      <label><input type="checkbox" defaultChecked /> Push</label>
      <label><input type="checkbox" /> Email</label>

      <br />

      <button>Изменить</button>
    </div>
  )
}