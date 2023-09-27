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
 * @property edition Количество экземпляров каталога.
 */
interface DistributionProps {
    ageGroup: string,
    town: string,
    gender: string,
    edition: number,
}

/**
 * Свойства каталога.
 * @property id Идентификатор.
 * @property creationDate Дата создания.
 * @property goods Набор товаров в каталоге.
 * @property count Тираж.
 * @property distributions Множество рассылок.
 */
interface BrochureProps {
    id: number,
    creationDate: string,
    goods: GoodProps[],
    count: number,
    distributions?: DistributionProps[],
}

export type {
    GoodProps,
    DistributionProps,
    BrochureProps
}