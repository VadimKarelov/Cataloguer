import { observable, action, makeAutoObservable } from "mobx";
import GoodsService from "../services/GoodsService";
import {GoodsProps} from "../types/BrochureTypes";

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
        const {data} = await this.getBrochureGoods(brochureId);
        if (!(data instanceof Array)) {
            this.isLoadingGoods = false;
            return;
        }

        console.log(data)
        this.goods = data;

        setTimeout(() => this.isLoadingGoods = false, 250);
    }
}

export default GoodsStore;