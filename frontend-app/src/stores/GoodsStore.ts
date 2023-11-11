import { observable, action, makeAutoObservable } from "mobx";
import GoodsService from "../services/GoodsService";
import {GoodsProps} from "../types/BrochureTypes";
import DistributionService from "../services/DistributionService";

/**
 * Класс хранилище для раздела товаров.
 */
class GoodsStore {
    /**
     * Множество товаров.
     */
    @observable public goods: GoodsProps[];

    /**
     * Загружаются ли товары.
     */
    @observable public isLoadingGoods: boolean;

    /**
     * Конструктор.
     */
    constructor() {
        this.goods = [];
        this.isLoadingGoods = false;

        makeAutoObservable(this);
        this.getBrochureGoods = this.getBrochureGoods.bind(this);
        this.updateCurrentBrochureGoods = this.updateCurrentBrochureGoods.bind(this);
        this.handleDeleteBrochureGood = this.handleDeleteBrochureGood.bind(this);
    }

    /**
     * Обарабывает удаление рассылки каталога.
     * @param goodId Идентификатор товара.
     * @param brochureId Идентификатор каталога.
     */
    @action public async handleDeleteBrochureGood(goodId: number, brochureId: number): Promise<string> {
        if (brochureId === -1 || goodId === -1) {
            return Promise.reject("Ошибка при удалении товара");
        }

        return await GoodsService.deleteBrochureGood(goodId, brochureId).then(
            async(response) => {
                const data = response.data;
                console.log(data);

                await this.updateCurrentBrochureGoods(brochureId);
                if (!isNaN(parseInt(data)) && parseInt(data) === -1) {
                    return Promise.reject("Ошибка при удалении товара");
                }
                return Promise.resolve("Рассылка товара удалёна успешно");
            },
            (error) => {
                console.log(error)
                return Promise.reject("Ошибка при удалении товара");
            }
        );
    }

    /**
     * Возвращает товары каталога по ИД каталога.
     * @param brochureId Идентификатор каталога.
     * @private
     */
    private async getBrochureGoods(brochureId: number) {
        return await GoodsService.getBrochureGoods(brochureId);
    }

    /**
     * Обновляет список товаров текущего каталога.
     * @param brochureId Идентификатор каталога.
     */
    @action public async updateCurrentBrochureGoods(brochureId: number) {
        this.isLoadingGoods = true;

        await this.getBrochureGoods(brochureId).then(
            (response) => {
                const data = response.data;
                if (!(data instanceof Array)) {
                    this.isLoadingGoods = false;
                    return;
                }

                this.goods = data;
            },
            (error) => console.log(error),
        ).finally(() => {
            setTimeout(() => this.isLoadingGoods = false, 250);
        });
    }
}

export default GoodsStore;