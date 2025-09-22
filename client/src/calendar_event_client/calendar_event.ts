export interface CalendarEvent {
  id: string
  title: string
  description: string
  startDate: Date
  endDate: Date
}

export interface GetCalendarEventsResponse {
  items: CalendarEvent[]
  total: number
}
