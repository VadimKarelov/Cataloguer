/**
 * Возвращает случайное число в промежутке от min до max.
 * @param min Минимальное число.
 * @param max Максимальное число.
 */
function random(min: number, max: number): number {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

export {random};