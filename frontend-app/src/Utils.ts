import {MetadataProps, MetadataTypes} from "./components/Operations/BrochureOperations/CreateBrochureButtonComponent";

/**
 * Возвращает случайное число в промежутке от min до max.
 * @param min Минимальное число.
 * @param max Максимальное число.
 */
export const random = (min: number, max: number): number => {
    return Math.floor(Math.random() * (max - min + 1)) + min;
};

/**
 * Возвращает результаты проверки полей.
 * @param _ Служебное поле.
 * @param value Значение поля.
 * @param itemMetadata Метаданные для поля.
 */
export const getValidator = (_: any, value: any, itemMetadata: MetadataProps) => {
    if (!value && itemMetadata.isRequired) return Promise.reject();

    let isFine: boolean = true;
    switch (itemMetadata.type) {
        case MetadataTypes.NMBR_FIELD: {
            if (isNaN(parseFloat(value))) return Promise.reject();

            const num = parseFloat(value);
            if (itemMetadata.min) {
                isFine = num >= itemMetadata.min;
            }

            if (itemMetadata.max) {
                isFine = num <= itemMetadata.max;
            }
        } break;
        case MetadataTypes.STR_FIELD: {
            const str: string = value;
            const length = str.length;
            if (itemMetadata.min) {
                isFine = length >= itemMetadata.min;
            }

            if (itemMetadata.max) {
                isFine = length <= itemMetadata.max;
            }
        } break;
    }

    return isFine ? Promise.resolve() : Promise.reject();
};