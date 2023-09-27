import { observable, action, makeAutoObservable } from "mobx"
import {BrochureProps, GoodProps} from "../types/BrochureTypes";
import {random} from "../Utils";

/**
 * Хранилище данных для каталога.
 */
class BrochureStore {
    /**
     * Выбран ли каталог.
     */
    @observable public isBrochureSelected: boolean;

    /**
     * Загружается ли каталог.
     */
    @observable public isBrochureLoading: boolean;

    /**
     * Загружается ли меню каталогов.
     */
    @observable public isBrochureMenuLoading: boolean;

    /**
     * Коллекция каталогов.
     */
    public brochures: BrochureProps[];

    /**
     * Текущий (выбранный) каталог.
     */
    public currentBrochure: BrochureProps | null;

    /**
     * Конструктор.
     */
    constructor() {
        this.brochures = [];
        this.currentBrochure = null;
        this.isBrochureSelected = false;
        this.isBrochureLoading = false;
        this.isBrochureMenuLoading = false;

        makeAutoObservable(this);

        this.loadBrochures = this.loadBrochures.bind(this);
        this.onBrochureClick = this.onBrochureClick.bind(this);
        this.initBrochures = this.initBrochures.bind(this);
        this.getRandomGoods = this.getRandomGoods.bind(this);
    }

    /**
     * Возвращает случайные товары.
     */
    private getRandomGoods(): GoodProps[] {
        const goodCount = 5;
        const names: Readonly<string[]> = [
            "iPad",
            "iPhone",
            "PC",
            "Monitor",
            "Mouse",
            "Keyboard",
            "Laptop"
        ];
        const goods: GoodProps[] = [];

        for (let i = 0; i < goodCount; i++) {
            const tempName = `${names.at(random(0, names.length - 1))}_${i}`;
            goods.push(
                {name: tempName, price: random(0, 10000)}
            );
        }

        return goods;
    }

    /**
     * Инициализирует коллекцию каталогов.
     */
    private initBrochures(): void {
        let tempBrochures: BrochureProps[] = [];
        const brochureCount = 100;
        for (let i = 0; i < brochureCount; ++i) {
            const tempCount = random(1, 5000);
            const tempCreationDate = new Date().toDateString();
            const tempGoods = this.getRandomGoods();
            tempBrochures.push(
                { id: i, count: tempCount, creationDate: tempCreationDate, goods: tempGoods }
            );
        }
        this.brochures = tempBrochures;
    }

    /**
     * Срабатывает после выбора каталога в меню.
     * @param brochureId - идентификатор каталога.
     */
    public onBrochureClick(brochureId: number): void {
        this.isBrochureSelected = true;
        this.isBrochureLoading = true;
        // some brochure load logic
        const foundBrochure = this.brochures.find(brochure => brochure.id === brochureId) ?? null;

        if (foundBrochure) {
            this.currentBrochure = foundBrochure;
        }

        //
        setTimeout(() => {
            this.isBrochureLoading = false;
        }, 1000);
    }

    /**
     * Загружает каталоги.
     */
    public loadBrochures() {
        this.isBrochureMenuLoading = true;

        //some load logic
        this.initBrochures();

        setTimeout(() => {
            this.isBrochureMenuLoading = false;
        }, 1000);
    }
}

export default BrochureStore;