import { formatDate } from "@angular/common";
import { Inject, Injectable, LOCALE_ID } from "@angular/core";
import { IStats } from "../models/IStats";
import { IAnalytics } from "../models/IAnalytics";
import { ICreatorAnalytics } from "../models/ICreatorAnalytics";
import { ICreatorStats } from "../models/ICreatorStats";
import { IPartnerAnalytics } from "../models/IPartnerAnalytics";
import { IPartnerStats } from "../models/IPartnerStats";
import { IUserStats } from "../models/IUserStats";

@Injectable({
    providedIn: 'root'
})
export class DateFormatter {

    constructor(@Inject(LOCALE_ID) public locale: string) {

    }

    public ParsePartnerAnalytics(partner: IPartnerAnalytics): IPartnerAnalytics {
        partner.lastMonth = ParseStatDates(partner.lastMonth) as IPartnerStats;
        partner.thisMonth = ParseStatDates(partner.thisMonth) as IPartnerStats;
        return partner;
    }

    public ParseCreatorAnalytics(creator: ICreatorAnalytics): ICreatorAnalytics {
        creator.lastMonth = ParseStatDates(creator.lastMonth) as ICreatorStats;
        creator.thisMonth = ParseStatDates(creator.thisMonth) as ICreatorStats;
        return creator;
    }

    public ParseUserAnalytics(user: IAnalytics): IAnalytics {
        user.lastMonth = ParseStatDates(user.lastMonth) as IUserStats;
        user.thisMonth = ParseStatDates(user.thisMonth) as IUserStats;
        return user;
    }

    public formatDatestamp(timestamp: Date): string {
        if (formatDate(new Date(), 'dd-MM-yyyy', this.locale) === formatDate(timestamp, 'dd-MM-yyyy', this.locale)) {
            return "Today";
        }
        return formatDate(timestamp, 'dd-MM-yyyy', this.locale);
    }

    public formatTimestamp(timestamp: Date): string {
        return formatDate(timestamp, 'HH:MM', this.locale);
    }

    public GetLastPeriodString(stats: IStats): string {
        return formatDate(stats.periodStart, 'dd-MM-yyyy', this.locale) + ' to ' + formatDate(stats.periodEnd, 'dd-MM-yyyy', this.locale);
    }
}

export function ParseStatDates(stats: IStats): IStats {
    stats.periodEnd = new Date(stats.periodEnd as string);
    stats.periodStart = new Date(stats.periodStart as string);
    return stats;
}
