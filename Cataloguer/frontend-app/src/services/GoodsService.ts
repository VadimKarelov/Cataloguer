import BaseService from "./BaseService";

/**
 * Класс сервис для обращению к контроллеру товаров каталога.
 */
class GoodsService extends BaseService {
    /**
     * Основной путь обращения к контроллеру.
     */
    private BACKEND_BASE_ROUTE: Readonly<string> = "/api/v1/goods";

    /**
     * Возвращает товары каталога по идентификатору каталога.
     * @param brochureId Идентифкиатор каталога.
     */
    public async getBrochureGoods(brochureId: number) {
        const methodRoute = `/getBrochureGoods/id=${brochureId}`;
        return await this.sendGetHttpRequest(this.BACKEND_BASE_ROUTE, methodRoute);
    }

    /**
     * Отправляет запрос на удаление товара из каталога.
     * @param goodId Идентификатор товара.
     * @param brochureId Идентификатор каталога.
     */
    public async deleteBrochureGood(goodId: number, brochureId: number) {
        const methodRoute = `/deleteBrochureGood/goodId=${goodId}/brochureId=${brochureId}`;
        return await this.sendGetHttpRequest(this.BACKEND_BASE_ROUTE, methodRoute);
    }
}

export default GoodsService;