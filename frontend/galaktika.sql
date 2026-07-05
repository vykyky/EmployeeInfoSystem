-- ============================================================
-- 1. Таблица Persons (физические лица)
-- ============================================================
CREATE TABLE Persons (
    Nrec        SERIAL PRIMARY KEY,
    FIO         VARCHAR(250) NOT NULL,          -- ФИО
    BornDate    DATE,                             -- Дата рождения
    Phone       VARCHAR(50),
    Email       VARCHAR(100)
);
COMMENT ON COLUMN Persons.Nrec IS 'Внутренний идентификатор';
COMMENT ON COLUMN Persons.FIO IS 'ФИО';
COMMENT ON COLUMN Persons.BornDate IS 'Дата рождения';

-- ============================================================
-- 2. Таблица Lschet (лицевые счета сотрудников)
-- ============================================================
CREATE TABLE Lschet (
    Nrec        SERIAL PRIMARY KEY,
    Tabn        VARCHAR(20) NOT NULL UNIQUE,     -- Табельный номер
    tPerson     INTEGER NOT NULL REFERENCES Persons(Nrec),
    DatPos      DATE,                            -- Дата принятия на работу
    Tarif       NUMERIC(12,2)                    -- Оклад
);
COMMENT ON COLUMN Lschet.Nrec IS 'Внутренний идентификатор';
COMMENT ON COLUMN Lschet.Tabn IS 'Табельный номер';
COMMENT ON COLUMN Lschet.tPerson IS 'Ссылка на Persons';
COMMENT ON COLUMN Lschet.DatPos IS 'Дата принятия на работу';
COMMENT ON COLUMN Lschet.Tarif IS 'Оклад';

-- ============================================================
-- 3. Таблица Catalogs (справочник - подразделения, должности, виды контрактов)
-- ============================================================
CREATE TABLE Catalogs (
    Nrec        SERIAL PRIMARY KEY,
    Name        VARCHAR(250) NOT NULL            -- Наименование
);
COMMENT ON COLUMN Catalogs.Nrec IS 'Идентификатор справочной записи';
COMMENT ON COLUMN Catalogs.Name IS 'Наименование';

-- ============================================================
-- 4. Таблица AppointMents (назначения / приказы / контракты)
-- ============================================================
CREATE TABLE AppointMents (
    Nrec            SERIAL PRIMARY KEY,
    Person          INTEGER NOT NULL REFERENCES Persons(Nrec),
    AppointDate     DATE,                        -- Дата назначения
    lPrizn          INTEGER,                     -- Признак (0 или 100)
    Department      INTEGER REFERENCES Catalogs(Nrec),
    Post            INTEGER REFERENCES Catalogs(Nrec),
    KindApp         INTEGER REFERENCES Catalogs(Nrec),
    OrderNmb        VARCHAR(50),                 -- Номер приказа
    OrderDate       DATE,                        -- Дата приказа
    ContractNmb     VARCHAR(50),                 -- Номер контракта
    ContractDate    DATE,                        -- Дата контракта
    DateEnd         DATE                         -- Дата окончания назначения (если есть)
);
COMMENT ON COLUMN AppointMents.Nrec IS 'Внутренний идентификатор';
COMMENT ON COLUMN AppointMents.Person IS 'Сотрудник (ссылка на Persons)';
COMMENT ON COLUMN AppointMents.AppointDate IS 'Дата назначения';
COMMENT ON COLUMN AppointMents.lPrizn IS 'Признак (0 или 100 для основных)';
COMMENT ON COLUMN AppointMents.Department IS 'Подразделение (Catalogs)';
COMMENT ON COLUMN AppointMents.Post IS 'Должность (Catalogs)';
COMMENT ON COLUMN AppointMents.KindApp IS 'Вид контракта (Catalogs)';
COMMENT ON COLUMN AppointMents.OrderNmb IS 'Номер приказа назначения';
COMMENT ON COLUMN AppointMents.OrderDate IS 'Дата приказа назначения';
COMMENT ON COLUMN AppointMents.ContractNmb IS 'Номер контракта';
COMMENT ON COLUMN AppointMents.ContractDate IS 'Дата контракта';
COMMENT ON COLUMN AppointMents.DateEnd IS 'Дата окончания назначения';



-- ============================================================
-- 5. Таблица WorkPeriod (рабочие периоды для отпусков)
-- ============================================================
CREATE TABLE WorkPeriod (
    Nrec        SERIAL PRIMARY KEY,
    dPerBeg     DATE,                            -- Дата начала периода
    dPerEnd     DATE                             -- Дата окончания периода
);
COMMENT ON COLUMN WorkPeriod.Nrec IS 'Идентификатор рабочего периода';
COMMENT ON COLUMN WorkPeriod.dPerBeg IS 'Дата начала периода, за который предоставляется отпуск';
COMMENT ON COLUMN WorkPeriod.dPerEnd IS 'Дата окончания периода';

-- ============================================================
-- 6. Таблица PutVacation (полагающиеся дни отпуска)
-- ============================================================
CREATE TABLE PutVacation (
    Nrec            SERIAL PRIMARY KEY,
    cWorkPeriod     INTEGER NOT NULL REFERENCES WorkPeriod(Nrec),
    wDayCount       NUMERIC(8,2),                -- Количество дней по норме
    ResDouble       INTEGER[]                    -- Массив [1] и [2] для корректировки
);
COMMENT ON COLUMN PutVacation.Nrec IS 'Внутренний идентификатор';
COMMENT ON COLUMN PutVacation.cWorkPeriod IS 'Ссылка на рабочий период';
COMMENT ON COLUMN PutVacation.wDayCount IS 'Полагается дней';
COMMENT ON COLUMN PutVacation.ResDouble IS 'Массив для корректировки (ResDouble[1], ResDouble[2])';

-- ============================================================
-- 7. Таблица FactOtpusk (фактические отпуска)
-- ============================================================
CREATE TABLE FactOtpusk (
    Nrec            SERIAL PRIMARY KEY,
    cLsch           INTEGER NOT NULL REFERENCES Lschet(Nrec),
    VacType         INTEGER,                     -- Тип отпуска (1 – основной)
    FactYearBeg     DATE,                        -- Дата начала отпуска
    FactYearend     DATE,                        -- Дата окончания отпуска
    Duration        NUMERIC(8,2),                -- Количество дней
    cWorkPeriod     INTEGER REFERENCES WorkPeriod(Nrec)
);
COMMENT ON COLUMN FactOtpusk.Nrec IS 'Внутренний идентификатор';
COMMENT ON COLUMN FactOtpusk.cLsch IS 'Ссылка на Lschet';
COMMENT ON COLUMN FactOtpusk.VacType IS 'Тип отпуска (1 – основной)';
COMMENT ON COLUMN FactOtpusk.FactYearBeg IS 'Дата начала отпуска';
COMMENT ON COLUMN FactOtpusk.FactYearend IS 'Дата окончания отпуска';
COMMENT ON COLUMN FactOtpusk.Duration IS 'Количество дней фактически';
COMMENT ON COLUMN FactOtpusk.cWorkPeriod IS 'Ссылка на рабочий период';

-- ============================================================
-- 8. Таблица PersCard (личная карточка сотрудника – размеры)
-- ============================================================
CREATE TABLE PersCard (
    Nrec        SERIAL PRIMARY KEY,
    cLschet     INTEGER NOT NULL UNIQUE REFERENCES Lschet(Nrec),
    Sizes       INTEGER[]                      -- [1]=рост, [2]=размер одежды, [3]=размер зимней одежды, [4]=размер обуви, [9]=размер зимней обуви
);
COMMENT ON COLUMN PersCard.Nrec IS 'Идентификатор личной карточки';
COMMENT ON COLUMN PersCard.cLschet IS 'Ссылка на сотрудника (Lschet)';
COMMENT ON COLUMN PersCard.Sizes IS 'Массив размеров: [1] рост, [2] одежда, [3] зимняя одежда, [4] обувь, [9] зимняя обувь';

-- ============================================================
-- 9. Таблица GroupSfo (группы спецодежды)
-- ============================================================
CREATE TABLE GroupSfo (
    Nrec        SERIAL PRIMARY KEY,
    Name        VARCHAR(250) NOT NULL,          -- Наименование группы
    Kod         VARCHAR(50),                    -- Код группы (для фильтрации)
    POSITIONS   INTEGER[]                       -- Массив флагов учета размеров ([1],[2],[3],[4],[9])
);
COMMENT ON COLUMN GroupSfo.Nrec IS 'Идентификатор группы';
COMMENT ON COLUMN GroupSfo.Name IS 'Наименование группы спецодежды';
COMMENT ON COLUMN GroupSfo.Kod IS 'Код группы';
COMMENT ON COLUMN GroupSfo.POSITIONS IS 'Флаги: 1 – использовать соответствующий размер из PersCard';



-- ============================================================
-- 10. Таблица PersSpec (нормы выдачи спецодежды по сотрудникам)
-- ============================================================
CREATE TABLE PersSpec (
    Nrec            SERIAL PRIMARY KEY,
    cPersCard       INTEGER NOT NULL REFERENCES PersCard(Nrec),
    cGroupSfo       INTEGER NOT NULL REFERENCES GroupSfo(Nrec),
    FrDate          DATE,                        -- Дата начала действия нормы
    toDate          DATE,                        -- Дата окончания (NULL – действует бесконечно)
    Kol             NUMERIC(12,3),               -- Количество по норме
    Srok            INTEGER,                     -- Срок (65535 = 'до износа')
    cMainSpec       INTEGER REFERENCES PersSpec(Nrec), -- Ссылка на основной вариант нормы (для замен)
    KindSpec        INTEGER                      -- Номер варианта замен (0 – основной, >0 – замена)
);
COMMENT ON COLUMN PersSpec.Nrec IS 'Идентификатор нормы';
COMMENT ON COLUMN PersSpec.cPersCard IS 'Ссылка на личную карточку';
COMMENT ON COLUMN PersSpec.cGroupSfo IS 'Группа спецодежды';
COMMENT ON COLUMN PersSpec.FrDate IS 'Дата начала действия нормы';
COMMENT ON COLUMN PersSpec.toDate IS 'Дата окончания (NULL – бессрочно)';
COMMENT ON COLUMN PersSpec.Kol IS 'Количество по норме';
COMMENT ON COLUMN PersSpec.Srok IS 'Срок (65535 – до износа)';
COMMENT ON COLUMN PersSpec.cMainSpec IS 'Ссылка на основную норму (для вариантов замен)';
COMMENT ON COLUMN PersSpec.KindSpec IS 'Номер варианта замен (0 – основная, >0 – вариант)';

-- ============================================================
-- 11. Таблица KatMbp (каталог МБП – наименования спецодежды в носке)
-- ============================================================
CREATE TABLE KatMbp (
    Nrec        SERIAL PRIMARY KEY,
    Name        VARCHAR(250) NOT NULL
);
COMMENT ON COLUMN KatMbp.Nrec IS 'Идентификатор вида спецодежды';
COMMENT ON COLUMN KatMbp.Name IS 'Наименование спецодежды в носке';

-- ============================================================
-- 12. Таблица PersSfo (выданная / эксплуатируемая спецодежда)
-- ============================================================
CREATE TABLE PersSfo (
    Nrec            SERIAL PRIMARY KEY,
    cPerscard       INTEGER NOT NULL REFERENCES PersCard(Nrec),
    cGroupSfo       INTEGER NOT NULL REFERENCES GroupSfo(Nrec),
    cKatMbp         INTEGER NOT NULL REFERENCES KatMbp(Nrec),
    CurKol          NUMERIC(12,3),               -- Количество в носке
    Spisdate        INTEGER,                     -- 0 – не списано, иначе дата списания (как число)
    GiveDate        DATE,                        -- Дата начала носки
    EndDate         DATE,                        -- Дата окончания носки
    Srok            INTEGER                      -- Срок (65535 – до износа)
);
COMMENT ON COLUMN PersSfo.Nrec IS 'Идентификатор выдачи';
COMMENT ON COLUMN PersSfo.cPerscard IS 'Сотрудник (личная карточка)';
COMMENT ON COLUMN PersSfo.cGroupSfo IS 'Группа спецодежды';
COMMENT ON COLUMN PersSfo.cKatMbp IS 'Вид спецодежды (каталог МБП)';
COMMENT ON COLUMN PersSfo.CurKol IS 'Количество в эксплуатации';
COMMENT ON COLUMN PersSfo.Spisdate IS 'Признак списания: 0 – не списано, иначе дата числом';
COMMENT ON COLUMN PersSfo.GiveDate IS 'Дата выдачи (начала носки)';
COMMENT ON COLUMN PersSfo.EndDate IS 'Дата окончания носки';
COMMENT ON COLUMN PersSfo.Srok IS 'Срок (65535 – до износа)';



-- ============================================================
-- 13. Таблица KatMc (каталог материальных ценностей)
-- ============================================================
CREATE TABLE KatMc (
    Nrec            SERIAL PRIMARY KEY,
    BarKod          VARCHAR(50),                 -- Штрихкод / код матценности
    Name            VARCHAR(250) NOT NULL,
    cGroupSfo       INTEGER NOT NULL REFERENCES GroupSfo(Nrec),
    Sizes           INTEGER[]                    -- Массив размеров (аналогично PersCard)
);
COMMENT ON COLUMN KatMc.Nrec IS 'Идентификатор матценности';
COMMENT ON COLUMN KatMc.BarKod IS 'Код матценности (штрихкод)';
COMMENT ON COLUMN KatMc.Name IS 'Наименование';
COMMENT ON COLUMN KatMc.cGroupSfo IS 'Группа спецодежды';
COMMENT ON COLUMN KatMc.Sizes IS 'Размеры ([1],[2],[3],[4],[9])';

-- ============================================================
-- 14. Таблица SaldoMc (остатки на складе)
-- ============================================================
CREATE TABLE SaldoMc (
    Nrec            SERIAL PRIMARY KEY,
    dSaldo          DATE NOT NULL,               -- Дата остатка
    cMol            INTEGER,                     -- Материально ответственное лицо (ссылка на Lschet?)
    cMC             INTEGER NOT NULL REFERENCES KatMc(Nrec),
    cPodr           INTEGER,                     -- Подразделение (возможно ссылка на Catalogs)
    cParty          INTEGER,                     -- Партия
    Kol             NUMERIC(12,3)                -- Количество
);
COMMENT ON COLUMN SaldoMc.Nrec IS 'Идентификатор записи остатка';
COMMENT ON COLUMN SaldoMc.dSaldo IS 'Дата остатка';
COMMENT ON COLUMN SaldoMc.cMol IS 'МОЛ (ссылка на Lschet, но не добавлено FK для упрощения)';
COMMENT ON COLUMN SaldoMc.cMC IS 'Материальная ценность (KatMc)';
COMMENT ON COLUMN SaldoMc.cPodr IS 'Подразделение (Catalogs)';
COMMENT ON COLUMN SaldoMc.cParty IS 'Партия';
COMMENT ON COLUMN SaldoMc.Kol IS 'Количество';

-- ============================================================
-- Дополнительные индексы для ускорения запросов
-- ============================================================
CREATE INDEX idx_lschet_tperson ON Lschet(tPerson);
CREATE INDEX idx_appointments_person ON AppointMents(Person);
CREATE INDEX idx_appointments_department ON AppointMents(Department);
CREATE INDEX idx_appointments_post ON AppointMents(Post);
CREATE INDEX idx_appointments_kindapp ON AppointMents(KindApp);
CREATE INDEX idx_factotpusk_clsch ON FactOtpusk(cLsch);
CREATE INDEX idx_factotpusk_vactype ON FactOtpusk(VacType);
CREATE INDEX idx_factotpusk_cworkperiod ON FactOtpusk(cWorkPeriod);
CREATE INDEX idx_putvacation_cworkperiod ON PutVacation(cWorkPeriod);
CREATE INDEX idx_perscard_clschet ON PersCard(cLschet);
CREATE INDEX idx_persspec_cperscard ON PersSpec(cPersCard);
CREATE INDEX idx_persspec_cgroupsfo ON PersSpec(cGroupSfo);
CREATE INDEX idx_persspec_cmainspec ON PersSpec(cMainSpec);
CREATE INDEX idx_perssfo_cperscard ON PersSfo(cPerscard);
CREATE INDEX idx_perssfo_cgroupsfo ON PersSfo(cGroupSfo);
CREATE INDEX idx_perssfo_ckatmbp ON PersSfo(cKatMbp);
CREATE INDEX idx_perssfo_spisdate ON PersSfo(Spisdate);
CREATE INDEX idx_katmc_cgroupsfo ON KatMc(cGroupSfo);
CREATE INDEX idx_saldomc_cmc ON SaldoMc(cMC);
CREATE INDEX idx_saldomc_cpodr ON SaldoMc(cPodr);
CREATE INDEX idx_saldomc_dsaldo ON SaldoMc(dSaldo);
