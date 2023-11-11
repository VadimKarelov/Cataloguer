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
import {SHOULD_USE_ONLY_DB_DATA} from "../constants/Routes";
import DistributionService from "../services/DistributionService";

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
        this.handleDeleteBrochure = this.handleDeleteBrochure.bind(this);
    }

    /**
     * Обработчик удаления каталога
     * @param brochureId Идентификатор каталога.
     */
    @action public async handleDeleteBrochure(brochureId: number): Promise<string> {
        if (brochureId === -1) {
            return Promise.reject("Ошибка при удалении каталога");
        }

        return await BrochureService.deleteBrochure(brochureId).then(
            async(response) => {
                await this.updateBrochureList();

                this.currentBrochure = null;
                this.saveBrochureToSessionStorage(null);

                const data = response.data;
                if (!isNaN(parseInt(data)) && parseInt(data) === -1) {
                    return Promise.reject("Ошибка при удалении каталога");
                }
                return Promise.resolve("Каталог удалён успешно");
            },
            (error) => {
                console.log(error);
                return Promise.reject("Ошибка при удалении каталога");
            }
        );
    }

    /**
     * Обрабатывает создание каталога.
     * @param brochure Каталог.
     */
    @action public async handleEditBrochure(brochure: EditBrochureHandlerProps): Promise<string> {
        const id = this.currentBrochure?.id ?? -1;
        const rejectReason = "Не удалось изменить каталог";
        if (id === -1) {
            return Promise.reject(rejectReason);
        }

        return await BrochureService.updateBrochure(id, brochure).then(
            (response) => {
                const data = response.data;

                const isResponseString = typeof data === "string";
                const isResponseNumber = typeof data === "number";
                const numberParseAttempt = parseInt(data);

                if (!isResponseString && !isResponseNumber) {
                    return Promise.reject(rejectReason);
                }

                if (isResponseString && !isNaN(numberParseAttempt) && numberParseAttempt === -1) {
                    return Promise.reject(rejectReason);
                }

                if (isResponseNumber && data === -1) {
                    return Promise.reject(rejectReason);
                }
                return Promise.resolve("Каталог успешно изменён");
            },
            (error) => {
                console.log(error);
                return Promise.reject(rejectReason);
            },
        );
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

        this.isBrochureLoading = true;
        this.isBrochureMenuLoading = true;
        const {data} = await BrochureService.createBrochure(brochure);

        const id = data;
        if (!(typeof id === "number") || id === -1) {
            return Promise.resolve("Ошибка при создании каталога");
        }

        const responses =  await Promise.all([
            BrochureService.getAllBrochures(),
            BrochureService.getBrochureById(id),
        ]);

        const brochuresAxiosResponse = responses[0];
        const brochureAxiosResponse = responses[1];

        if (brochuresAxiosResponse && brochuresAxiosResponse.data instanceof Array) {
            this.brochures = brochuresAxiosResponse.data;
        }

        if (brochureAxiosResponse) {
            await this.onBrochureClick(id);
            // this.currentBrochure = brochureAxiosResponse.data;
        }

        this.isBrochureLoading = false;
        this.isBrochureMenuLoading = false;
        return Promise.resolve("Каталог успешно создан");
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
            (error) => console.log(error)
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
            if (SHOULD_USE_ONLY_DB_DATA === "false") {
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
        await BrochureService.getAllBrochures().then(
            (response) => {
                const data = response.data;
                if (!(data instanceof Array)) return;

                this.brochures = data;
            },
            (error) => console.log(error)
        );
    }

    /**
     * Загружает каталоги.
     */
    @action public loadBrochures() {
        this.isBrochureMenuLoading = true;

        SHOULD_USE_ONLY_DB_DATA === "false" ? this.initBrochures() : this.updateBrochureList();

        setTimeout(() => {
            this.isBrochureMenuLoading = false;
        }, 300);
    }
}

export default BrochureStore;