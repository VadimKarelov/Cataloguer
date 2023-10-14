import BrochureStore, {Status} from "../stores/BrochureStore";

/**
 * Свойства товара.
 * @property name Название.
 * @property price Цена.
 */
interface GoodProps {
    name: string,
    price: number,
}

/**
 * Свойства рассылки.
 * @property ageGroup Возрастная группа.
 * @property town Населённый пункт.
 * @property gender Пол.
 * @property count Количество экземпляров каталога.
 */
interface DistributionProps {
    ageGroup: string,
    town: string,
    gender: string,
    count: number,
}

/**
 * Свойства каталога.
 * @property id Идентификатор.
 * @property name Наименование каталога.
 * @property creationDate Дата создания.
 * @property goods Набор товаров в каталоге.
 * @property edition Тираж.
 * @property status Статус.
 * @property distributions Множество рассылок.
 */
interface BrochureProps {
    id: number,
    name?: string,
    creationDate: string,
    goods: GoodProps[],
    edition: number,
    status: string,
    distributions?: DistributionProps[],
}

/**
 * Базовые свойства компонентов.
 * @param brochureStore Хранилище каталогов.
 */
interface BaseStoreInjector {
    brochureStore?: BrochureStore,
}

/**
 * Тип для получения данных по ключу.
 */
type customProp = {
    [key: string]: any
};

/**
 * Свойства метаданных каталога.
 * @param name Название поля.
 * @param displayName Отображаемое название.
 * @param order Порядок следования.
 * @param isVisible Видно ли поле.
 */
interface BrochureBaseInfoProps {
    name: string,
    displayName: string,
    order: number,
    isVisible?: boolean
}

/**
 * Свойства для набора статусов.
 * @param key Ключ (перечислимый тип).
 * @param value Значение.
 */
interface StatusArrayProps {
    key: Status,
    value: string
}

export type {
    GoodProps,
    DistributionProps,
    BrochureProps,
    customProp,
    BaseStoreInjector,
    BrochureBaseInfoProps,
    StatusArrayProps,
}