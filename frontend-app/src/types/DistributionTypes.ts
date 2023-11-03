/**
 * Свойства возрастной группы из БД.
 * @param id Идентификатор.
 * @param description Описание.
 * @param minimalAge Минимальный возраст для группы.
 * @param maximalAge Максимальный возраст для группы.
 */
export interface AgeGroupDbProps {
    id: number,
    description: string,
    minimalAge: number,
    maximalAge: number,
}

/**
 * Свойства города из БД.
 * @param id Идентификатор.
 * @param name Наименование.
 * @param population Число жителей.
 */
export interface TownDbProps {
    id: number,
    name: string,
    population: number,
}

/**
 * Свойства пола из БД.
 * @param id Идентификатор.
 * @param name Наименование.
 */
export interface GenderDbProps {
    id: number,
    name: string,
}

/**
 * Свойства параметров запроса создания рассылки.
 * @param ageGroupId Идентификатор возрастной группы.
 * @param genderId Идентификатор пола.
 * @param townId Идентификатор города.
 * @param brochureCount Тираж рассылки.
 */
export interface DistributionDbProps {
    ageGroupId: number,
    genderId: number,
    townId: number,
    brochureCount: number,
}

/**
 * Свойства параметров запроса создания рассылки.
 * @param brochureId Идентификатор каталога.
 */
export interface CreateDistributionDbProps extends DistributionDbProps {
    brochureId: number,
}

/**
 * Свойства параметров запроса редактирования рассылки.
 * @param id Идентификатор рассылки.
 */
export interface EditDistributionDbProps extends CreateDistributionDbProps {
    id: number,
}

// export interface DistributionDbProps extends CreateDistributionDbProps {
//     brochureName: string,
//     ageGroupName: string,
//     genderName: string,
//     townName: string,
// }