import { observable, action, makeAutoObservable } from "mobx"
import {BrochureProps, DistributionProps, GoodProps} from "../types/BrochureTypes";
import {random} from "../Utils";
import DistributionService from "../services/DistributionService";

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
        this.getRandomDistributions = this.getRandomDistributions.bind(this);

        this.getAgeGroups = this.getAgeGroups.bind(this);
        this.updateDistributionLists = this.updateDistributionLists.bind(this);
    }

    /**
     * Возвращает возрастные группы.
     */
    private async getAgeGroups() {
        return await DistributionService.getAgeGroups();
    }

    /**
     * Обновляет список возврастных группы.
     */
    public updateDistributionLists() {
        this.getAgeGroups().then(resp => {
           console.log(resp)
        });
    }

    /**
     * Возвращает случайные товары.
     */
    private getRandomGoods(): GoodProps[] {
        const goodCount = 10;
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
     * Возвращает случайные рассылки.
     * @private
     */
    private getRandomDistributions(): DistributionProps[] {
        const brochureCount = 20;
        const towns: Readonly<string[]> = [
            "Пермь",
            "Москва",
            "Псков",
            "Нальчик",
            "Казань",
            "Тюмень",
            "Екатеринбург"
        ];

        const genders: Readonly<string[]> = [
            "Мужчина",
            "Женщина",
            "Не выбрано",
        ];

        const ageGroups: Readonly<string[]> = [
            "Пожилые",
            "Взрослые",
            "Молодые",
        ];

        const distributions: DistributionProps[] = [];

        for (let i = 0; i < brochureCount; i++) {
            const town = towns[random(0, towns.length - 1)];
            const ageGroup = ageGroups[random(0, ageGroups.length - 1)];
            const gender = genders[random(0, genders.length - 1)];
            distributions.push(
                {count: i + brochureCount, town: town, gender: gender, ageGroup: ageGroup}
            );
        }

        return distributions;
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
            const tempDistributions = this.getRandomDistributions();
            tempBrochures.push(
                { id: i, name: `Каталог ${i}`, edition: tempCount, creationDate: tempCreationDate, goods: tempGoods, distributions: tempDistributions }
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