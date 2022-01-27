import { IPartnerStats } from "./IPartnerStats";

export interface IPartnerAnalytics {
    lastMonth: IPartnerStats;
    thisMonth: IPartnerStats;
}
