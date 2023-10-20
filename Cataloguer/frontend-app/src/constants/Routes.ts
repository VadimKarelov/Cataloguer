
/**
 * Адрес backend.
 */
export const BACKEND_ROUTE: Readonly<string> = process.env.REACT_APP_BACKEND_ROUTE ?? "";

/**
 * Адрес frontend.
 */
export const FRONTEND_ROUTE: Readonly<string> = process.env.REACT_APP_FRONTEND_ROUTE ?? "";

/**
 * Основной путь обращения к контроллеру.
 */
export const BACKEND_CONTROLLER_ROUTE: Readonly<string> = `${BACKEND_ROUTE}/api/v1/cataloguer`;