/**
 * Следует ли использовать данные из БД.
 */
export const SHOULD_USE_ONLY_DB_DATA: Readonly<string> = process.env.REACT_APP_SHOULD_USE_ONLY_DB_DATA ?? "false";
/**
 * Следует ли использовать данные из БД.
 */
export const IS_DEBUG: Readonly<string> = process.env.REACT_APP_DEBUG ?? "true";