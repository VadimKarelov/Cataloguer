import BrochureStore from "../stores/BrochureStore";

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
 * @property distributions Множество рассылок.
 */
interface BrochureProps {
    id: number,
    name?: string,
    creationDate: string,
    goods: GoodProps[],
    edition: number,
    distributions?: DistributionProps[],
}

interface BaseStoreInjector {
    brochureStore?: BrochureStore,
}

type customProp = {
    [key: string]: any
};

interface BrochureBaseInfoProps {
    name: string,
    displayName: string,
    order: number,
    isVisible?: boolean
}

export type {
    GoodProps,
    DistributionProps,
    BrochureProps,
    customProp,
    BaseStoreInjector,
    BrochureBaseInfoProps
}