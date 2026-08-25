-- ============================================================
-- 1. Users — пользователи приложения (логин, роль, настройки)
-- ============================================================
CREATE TABLE Users (
    id              SERIAL PRIMARY KEY,
    tabn            VARCHAR(20) NOT NULL UNIQUE,    -- Табельный номер (связка с Галактикой)
    password_hash   VARCHAR(255) NOT NULL,
    role            VARCHAR(20) NOT NULL DEFAULT 'employee'
                        CHECK (role IN ('employee', 'manager', 'admin')),
    push_token      VARCHAR(500),                   -- FCM/APNs токен для push-уведомлений
    notify_push     BOOLEAN NOT NULL DEFAULT TRUE,
    notify_sms      BOOLEAN NOT NULL DEFAULT FALSE,
    notify_email    BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMP NOT NULL DEFAULT NOW(),
    last_login_at   TIMESTAMP
);

COMMENT ON TABLE Users IS 'Пользователи приложения';
COMMENT ON COLUMN Users.tabn IS 'Табельный номер — ключ связки с ИС Галактика';
COMMENT ON COLUMN Users.role IS 'Роль: employee — сотрудник, manager — менеджер, admin — администратор';
COMMENT ON COLUMN Users.push_token IS 'Токен устройства для push-уведомлений (FCM/APNs)';

-- ============================================================
-- 2. EmployeeCache — кэш личных данных сотрудника из Галактики
-- ============================================================
CREATE TABLE EmployeeCache (
    id              SERIAL PRIMARY KEY,
    tabn            VARCHAR(20) NOT NULL UNIQUE REFERENCES Users(tabn),
    fio             VARCHAR(250),
    born_date       DATE,
    hire_date       DATE,
    department      VARCHAR(250),
    post            VARCHAR(250),
    phone           VARCHAR(50),
    email           VARCHAR(100),
    size_clothes        INTEGER,                    -- Размер одежды
    size_clothes_winter INTEGER,                    -- Размер зимней одежды
    size_shoes          INTEGER,                    -- Размер обуви
    size_shoes_winter   INTEGER,                    -- Размер зимней обуви
    height              INTEGER,                    -- Рост
    synced_at       TIMESTAMP                       -- Время последней синхронизации
);

COMMENT ON TABLE EmployeeCache IS 'Кэш личных данных сотрудников, синхронизируется из ИС Галактика';
COMMENT ON COLUMN EmployeeCache.tabn IS 'Табельный номер — связка с Users и Галактикой';
COMMENT ON COLUMN EmployeeCache.synced_at IS 'Дата и время последней синхронизации с Галактикой';

-- ============================================================
-- 3. PaySlips — расчётные листки (файлы с сервера Галактики)
-- ============================================================
CREATE TABLE PaySlips (
    id              SERIAL PRIMARY KEY,
    tabn            VARCHAR(20) NOT NULL REFERENCES Users(tabn),
    period_month    SMALLINT NOT NULL CHECK (period_month BETWEEN 1 AND 12),
    period_year     SMALLINT NOT NULL,
    file_path       VARCHAR(500) NOT NULL,          -- Путь к HTML-файлу на сервере приложения
    created_at      TIMESTAMP NOT NULL DEFAULT NOW(),
    UNIQUE (tabn, period_month, period_year)
);

COMMENT ON TABLE PaySlips IS 'Расчётные листки сотрудников (HTML-файлы, перемещённые с сервера Галактики)';
COMMENT ON COLUMN PaySlips.file_path IS 'Путь к файлу формата ФИО_табном_месяц_год.html на сервере приложения';

-- ============================================================
-- 4. PPECache — кэш выданной спецодежды из Галактики
-- ============================================================
CREATE TABLE PPECache (
    id              SERIAL PRIMARY KEY,
    tabn            VARCHAR(20) NOT NULL REFERENCES Users(tabn),
    group_name      VARCHAR(250),                   -- Наименование группы спецодежды
    item_name       VARCHAR(250),                   -- Наименование позиции
    give_date       DATE,                           -- Дата выдачи
    end_date        DATE,                           -- Дата окончания носки
    wear_period     INTEGER,                        -- Срок носки (65535 = до износа)
    quantity        NUMERIC(12, 3),                 -- Количество
    synced_at       TIMESTAMP                       -- Время последней синхронизации
);

COMMENT ON TABLE PPECache IS 'Кэш выданной спецодежды, синхронизируется из ИС Галактика';
COMMENT ON COLUMN PPECache.wear_period IS 'Срок носки в месяцах; 65535 — до износа';
COMMENT ON COLUMN PPECache.synced_at IS 'Дата и время последней синхронизации с Галактикой';

-- ============================================================
-- 5. News — новости предприятия (создаются менеджером)
-- ============================================================
CREATE TABLE News (
    id              SERIAL PRIMARY KEY,
    title           VARCHAR(500) NOT NULL,
    body            TEXT NOT NULL,
    image_path      VARCHAR(500),                   -- Путь к загруженному изображению
    author_id       INTEGER NOT NULL REFERENCES Users(id),
    is_published    BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMP NOT NULL DEFAULT NOW(),
    published_at    TIMESTAMP
);

COMMENT ON TABLE News IS 'Новости предприятия, публикуются менеджером';
COMMENT ON COLUMN News.image_path IS 'Путь к фото статьи на сервере приложения';
COMMENT ON COLUMN News.is_published IS 'FALSE — черновик, TRUE — опубликована и видна всем';

-- ============================================================
-- 6. RequestTypes — справочник типов электронных запросов
-- ============================================================
CREATE TABLE RequestTypes (
    id              SERIAL PRIMARY KEY,
    name            VARCHAR(250) NOT NULL,
    is_active       BOOLEAN NOT NULL DEFAULT TRUE   -- FALSE — скрыт из списка у сотрудника
);

COMMENT ON TABLE RequestTypes IS 'Справочник типов электронных запросов, управляется администратором';
COMMENT ON COLUMN RequestTypes.is_active IS 'Если FALSE — тип скрыт из выпадающего списка в приложении';

-- ============================================================
-- 7. Requests — электронные запросы от сотрудников
-- ============================================================
CREATE TABLE Requests (
    id                  SERIAL PRIMARY KEY,
    employee_id         INTEGER NOT NULL REFERENCES Users(id),
    request_type_id     INTEGER NOT NULL REFERENCES RequestTypes(id),
    comment             TEXT,                       -- Комментарий сотрудника
    new_value           VARCHAR(500),               -- Новое значение (для запросов на изменение данных)
    status              VARCHAR(20) NOT NULL DEFAULT 'accepted'
                            CHECK (status IN ('accepted', 'assigned', 'in_progress', 'done')),
    manager_id          INTEGER REFERENCES Users(id), -- Назначенный менеджер
    resolution_comment  TEXT,                       -- Описание решения от менеджера
    created_at          TIMESTAMP NOT NULL DEFAULT NOW(),
    resolved_at         TIMESTAMP
);

COMMENT ON TABLE Requests IS 'Электронные запросы сотрудников — задачи для менеджера';
COMMENT ON COLUMN Requests.new_value IS 'Новое значение при запросах на изменение (размер одежды, телефон и т.д.)';
COMMENT ON COLUMN Requests.status IS 'accepted — принята, assigned — назначена, in_progress — в работе, done — выполнена';
COMMENT ON COLUMN Requests.manager_id IS 'Менеджер, которому назначена задача (назначает администратор)';

-- ============================================================
-- 8. Notifications — уведомления пользователей
-- ============================================================
CREATE TABLE Notifications (
    id              SERIAL PRIMARY KEY,
    recipient_id    INTEGER NOT NULL REFERENCES Users(id),
    sender_id       INTEGER REFERENCES Users(id),   -- NULL — системное уведомление
    title           VARCHAR(500) NOT NULL,
    body            TEXT,
    type            VARCHAR(10) NOT NULL DEFAULT 'push'
                        CHECK (type IN ('push', 'sms', 'email', 'in_app')),
    is_read         BOOLEAN NOT NULL DEFAULT FALSE,
    created_at      TIMESTAMP NOT NULL DEFAULT NOW()
);

COMMENT ON TABLE Notifications IS 'Уведомления пользователей (push, sms, email, внутренние)';
COMMENT ON COLUMN Notifications.sender_id IS 'Отправитель; NULL — системное уведомление';
COMMENT ON COLUMN Notifications.type IS 'Канал доставки: push, sms, email, in_app';

-- ============================================================
-- Индексы
-- ============================================================
CREATE INDEX idx_employeecache_tabn       ON EmployeeCache(tabn);
CREATE INDEX idx_payslips_tabn            ON PaySlips(tabn);
CREATE INDEX idx_payslips_period          ON PaySlips(period_year, period_month);
CREATE INDEX idx_ppecache_tabn            ON PPECache(tabn);
CREATE INDEX idx_news_author              ON News(author_id);
CREATE INDEX idx_news_published           ON News(is_published, published_at DESC);
CREATE INDEX idx_requests_employee        ON Requests(employee_id);
CREATE INDEX idx_requests_manager         ON Requests(manager_id);
CREATE INDEX idx_requests_status          ON Requests(status);
CREATE INDEX idx_requests_type            ON Requests(request_type_id);
CREATE INDEX idx_notifications_recipient  ON Notifications(recipient_id);
CREATE INDEX idx_notifications_unread     ON Notifications(recipient_id, is_read) WHERE is_read = FALSE;