import { observable, action, makeAutoObservable } from "mobx";
import {
    BrochureProps, CreateBrochureHandlerProps,
    DistributionProps, EditBrochureHandlerProps, GoodDBProps,
    GoodProps,
    GoodsExtendedProps,
    GoodsProps,
    StatusArrayProps
} from "../types/BrochureTypes";
import {random} from "../Utils";
import {ageGroups, genders, goodsNames, towns} from "./HandbookExamples";
import GoodsService from "../services/GoodsService";
import BrochureService from "../services/BrochureService";
import moment from "moment";

/**
 * Следует ли использовать данные из БД.
 */
const shouldUseOnlyDbData: Readonly<string> = process.env.REACT_APP_SHOULD_USE_ONLY_DB_DATA ?? "false";

/**
 * Путь к данным каталога в session storage.
 */
const BROCHURE_SS_PATH: Readonly<string> = "ss_brochure";

/**
 * Перечислимый тип для статусов.
 */
export enum Status {
    EFFECTIVE,
    INEFFECTIVE,
    UNCHECKED,
}

/**
 * Набор возможных статусов.
 */
export const BROCHURE_STATUSES: Readonly<StatusArrayProps[]> = [
    {key: Status.UNCHECKED, value: "Не проверен"},
    {key: Status.INEFFECTIVE, value: "Не эффективен"},
    {key: Status.EFFECTIVE, value: "Эффективный"},
];

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
     * Все возможные товары.
     * Необходимы в форме создания каталога.
     */
    public allGoods: GoodsProps[];

    /**
     * Отмеченные товары.
     */
    private checkedGoods: GoodDBProps[];

    /**
     * Загружаются ли товары.
     */
    public isLoadingGoods: boolean;

    /**
     * Конструктор.
     */
    constructor() {
        this.brochures = [];
        this.allGoods = [];
        this.checkedGoods = [];
        this.currentBrochure = null;
        this.isBrochureSelected = false;
        this.isBrochureLoading = false;
        this.isBrochureMenuLoading = false;
        this.isLoadingGoods = false;

        makeAutoObservable(this);

        this.loadBrochures = this.loadBrochures.bind(this);
        this.onBrochureClick = this.onBrochureClick.bind(this);
        this.initBrochures = this.initBrochures.bind(this);
        this.getRandomGoods = this.getRandomGoods.bind(this);
        this.getRandomDistributions = this.getRandomDistributions.bind(this);
        this.getSavedBrochure = this.getSavedBrochure.bind(this);
        this.saveBrochureToSessionStorage = this.saveBrochureToSessionStorage.bind(this);
        this.getSavedBrochureMenu = this.getSavedBrochureMenu.bind(this);
        this.reset = this.reset.bind(this);

        this.setCheckedGoods = this.setCheckedGoods.bind(this);
        this.updateGoodsList = this.updateGoodsList.bind(this);

        this.handleEditBrochure = this.handleEditBrochure.bind(this);
        this.handleCreateBrochure = this.handleCreateBrochure.bind(this);
        this.getBrochureById = this.getBrochureById.bind(this);
        this.updateBrochureList = this.updateBrochureList.bind(this);
    }

    /**
     * Обрабатывает создание каталога.
     * @param brochure Каталог.
     */
    @action public async handleEditBrochure(brochure: EditBrochureHandlerProps) {
        const id = this.currentBrochure?.id ?? -1;
        if (id === -1) return;

        console.log("Отправляем на backend: ", brochure);

        await BrochureService.updateBrochure(id, brochure).then((response: {data: any}) => {
            console.log("Получаем с backend: ", response.data)
        });
    }

    /**
     * Вызывает запрос на получения данных о каталоге по идентификатору.
     * @param id Идентификатор каталога.
     */
    @action public async getBrochureById(id: number) {
        return await BrochureService.getBrochureById(id);
    }

    /**
     * Обрабатывает создание каталога.
     * @param brochure Каталог.
     */
    @action public async handleCreateBrochure(brochure: CreateBrochureHandlerProps) {
        brochure.positions = [...this.checkedGoods.map(good => ({...good}))];

        console.log("Отправляем на backend: ", brochure);

        this.isBrochureLoading = true;
        this.isBrochureMenuLoading = true;
        await BrochureService.createBrochure(brochure).then(async (response: {data: any}) => {
            const id = response.data;

            console.log("Получаем с backend id каталога: ", id)

            if (!(typeof id === "number")) return;

            await Promise.all([
                BrochureService.getAllBrochures(),
                BrochureService.getBrochureById(id),
            ])
                .then((responses) => {
                    const brochuresAxiosResponse = responses[0];
                    const brochureAxiosResponse = responses[1];

                    if (brochuresAxiosResponse && brochuresAxiosResponse.data instanceof Array) {
                        this.brochures = brochuresAxiosResponse.data;
                    }

                    if (brochureAxiosResponse) {
                        this.onBrochureClick(id);
                        // this.currentBrochure = brochureAxiosResponse.data;
                    }
                })
                .finally(() => {
                    this.isBrochureLoading = false;
                    this.isBrochureMenuLoading = false;
                });
        });
    }

    /**
     * Сохраняет отмеченные строки.
     * @param goods Товары.
     */
    @action public setCheckedGoods(goods: GoodsExtendedProps[]) {
        this.checkedGoods = goods
            .filter(good => good.isChecked)
            .map((good) => ({id: good.id, price: parseFloat(good.price.toString())}));
    }

    /**
     * Обновляет список товаров для создания каталога.
     */
    @action public updateGoodsList() {
        this.allGoods = [];
        this.checkedGoods = [];

        this.isLoadingGoods = true;
        GoodsService.getGoods().then(
            ((response: {data: any}) => {
                const isFine = response.data instanceof Array;
                if (!isFine) return;

                this.allGoods = response.data;
            }),
            (err) => console.log(err)
        ).finally(() => this.isLoadingGoods = false);
    }

    /**
     * Очищает данные о каталоге.
     */
    @action public reset(): void {
        this.currentBrochure = null;
        sessionStorage.removeItem(BROCHURE_SS_PATH);
    }

    /**
     * Возвращает случайные товары.
     */
    private getRandomGoods(): GoodProps[] {
        const goodCount = 10;
        const goods: GoodProps[] = [];

        for (let i = 0; i < goodCount; i++) {
            const tempName = `${goodsNames.at(random(0, goodsNames.length - 1))}_${i}`;
            goods.push(
                {name: tempName, price: random(0, 10000)}
            );
        }

        return goods;
    }

    /**
     * Возвращает случайные рассылки.
     */
    private getRandomDistributions(): DistributionProps[] {
        const brochureCount = 20;
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
            const tempCreationDate = moment(new Date()).toDate().toDateString();
            const tempGoods = this.getRandomGoods();
            const tempDistributions = this.getRandomDistributions();
            const status = BROCHURE_STATUSES[random(0, BROCHURE_STATUSES.length - 1)].value;
            tempBrochures.push(
                { id: i, name: `Каталог ${i}`, edition: tempCount, date: tempCreationDate, goods: tempGoods, distributions: tempDistributions, status: status }
            );
        }
        this.brochures = tempBrochures;
    }

    /**
     * Возвращает меню с открытыми каталогами из session storage.
     */
    @action public getSavedBrochureMenu(): string[] {
        const brochure = this.getSavedBrochure();
        return brochure ? [`brochure_${brochure.id}`] : [];
    }

    /**
     * Возвращает сохранённый каталог в сессии браузера.
     */
    @action public getSavedBrochure(): BrochureProps | null {
        const json = sessionStorage.getItem(BROCHURE_SS_PATH);
        if (json === null || json === "undefined") return null;
        return JSON.parse(json);
    }

    /**
     * Сохраняет каталог  в session storage.
     * @param brochure - каталог.
     */
    @action private saveBrochureToSessionStorage(brochure: BrochureProps | null): void {
        sessionStorage.setItem(BROCHURE_SS_PATH, JSON.stringify(brochure));
    }

    /**
     * Срабатывает после выбора каталога в меню.
     * @param brochureId - идентификатор каталога.
     */
    @action public async onBrochureClick(brochureId: number) {
        if (isNaN(brochureId) || brochureId === -1) return;

        this.isBrochureSelected = true;
        this.isBrochureLoading = true;

        let foundBrochure = null;

        // if (brochureId !== -1) {
            if (shouldUseOnlyDbData === "false") {
                foundBrochure = this.brochures.find(brochure => brochure.id === brochureId) ?? null;
            } else {
                await this.getBrochureById(brochureId).then((response: {data: any}) => {
                    foundBrochure = response.data;
                });
            }
        // }

        this.currentBrochure = foundBrochure;
        this.saveBrochureToSessionStorage(foundBrochure);

        setTimeout(() => {
            this.isBrochureLoading = false;
        }, 250);
    }

    /**
     * Обновляет список каталогов.
     * @private
     */
    @action private async updateBrochureList() {
        const {data} = await BrochureService.getAllBrochures();
        if (!(data instanceof Array)) return;

        this.brochures = data;
    }

    /**
     * Загружает каталоги.
     */
    @action public loadBrochures() {
        this.isBrochureMenuLoading = true;

        shouldUseOnlyDbData === "false" ? this.initBrochures() : this.updateBrochureList();

        setTimeout(() => {
            this.isBrochureMenuLoading = false;
        }, 500);
    }
}

export default BrochureStore;