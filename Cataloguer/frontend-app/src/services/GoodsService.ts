import BaseService from "./BaseService";

/**
 * Адрес backend.
 */
const BACKEND_ROUTE: Readonly<string> = process.env.BACKEND_ROUTE ?? "";

/**
 * Основной путь обращения к контроллеру.
 */
const BACKEND_CONTROLLER_ROUTE: Readonly<string> = `${BACKEND_ROUTE}/api/v1/goods`;

/**
 * Класс сервис для обращению к контроллеру товаров каталога.
 */
class GoodsService extends BaseService {
    /**
     * Возвращает товары каталога по идентификатору каталога.
     * @param brochureId Идентифкиатор каталога.
     */
    public static async getBrochureGoods(brochureId: number) {
        const methodRoute = `/getBrochureGoods/id=${brochureId}`;
        return await BaseService.sendGetHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute);
    }

    /**
     * Отправляет запрос на удаление товара из каталога.
     * @param goodId Идентификатор товара.
     * @param brochureId Идентификатор каталога.
     */
    public static async deleteBrochureGood(goodId: number, brochureId: number) {
        const methodRoute = `/deleteBrochureGood/goodId=${goodId}/brochureId=${brochureId}`;
        return await BaseService.sendGetHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute);
    }
}

export default GoodsService;