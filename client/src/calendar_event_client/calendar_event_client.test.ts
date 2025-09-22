import { CalendarEventClient } from './calendar_event_client'
import type {
  CalendarEvent,
  GetCalendarEventsResponse,
} from './calendar_event.ts'
import { type Mock, vi } from 'vitest'

describe('CalendarEventClient', async () => {
  const baseUrl = 'http://localhost:3000'
  let client: CalendarEventClient

  beforeEach(() => {
    client = new CalendarEventClient(baseUrl)
    vi.clearAllMocks()
  })

  describe('getCalendarEventById', async () => {
    it('should fetch a calendar event by ID', async () => {
      const mockEvent: CalendarEvent = {
        id: 'someId',
        title: 'someTitle',
        description: 'someDescription',
        startDate: new Date(2025, 9, 21, 12, 0, 0, 0),
        endDate: new Date(2025, 9, 21, 13, 0, 0, 0),
      }
      global.fetch = vi.fn(() =>
        Promise.resolve({
          ok: true,
          json: () => Promise.resolve(mockEvent),
        })
      ) as Mock

      const event = await client.getCalendarEventById(mockEvent.id)
      expect(event).toEqual(mockEvent)
      expect(fetch).toHaveBeenCalledWith(
        `${baseUrl}/api/Events/${mockEvent.id}`,
        {
          method: 'GET',
          headers: { 'Content-Type': 'application/json' },
        }
      )
    })

    it('should throw an error if the response is not ok', async () => {
      const expectedError = new Error('Some error')
      global.fetch = vi.fn(() =>
        Promise.resolve({
          ok: false,
          statusText: expectedError.message,
        })
      ) as Mock

      await expect(client.getCalendarEventById('1')).rejects.toThrow(
        expectedError.message
      )
    })
  })

  describe('getCalendarEvents', async () => {
    it('should fetch calendar events with pagination', async () => {
      const mockResponse: GetCalendarEventsResponse = {
        items: [
          {
            id: 'someId',
            title: 'someTitle',
            description: 'someDescription',
            startDate: new Date(2025, 9, 21, 12, 0, 0, 0),
            endDate: new Date(2025, 9, 21, 13, 0, 0, 0),
          },
          {
            id: 'someId2',
            title: 'someTitle2',
            description: 'someDescription2',
            startDate: new Date(2025, 9, 21, 14, 0, 0, 0),
            endDate: new Date(2025, 9, 21, 15, 0, 0, 0),
          },
        ],
        total: 0,
      }
      global.fetch = vi.fn(() =>
        Promise.resolve({
          ok: true,
          json: () => Promise.resolve(mockResponse),
        })
      ) as Mock

      const expectedSkip = 0
      const expectedTop = 10
      const events = await client.getCalendarEvents(expectedSkip, expectedTop)
      expect(events).toEqual(mockResponse)
      expect(fetch).toHaveBeenCalledWith(
        `${baseUrl}/api/Events?skip=${expectedSkip}&top=${expectedTop}`,
        {
          method: 'GET',
          headers: { 'Content-Type': 'application/json' },
        }
      )
    })

    it('should throw an error if the response is not ok', async () => {
      const expectedError = new Error('Some error')
      global.fetch = vi.fn(() =>
        Promise.resolve({
          ok: false,
          statusText: expectedError.message,
        })
      ) as Mock

      await expect(client.getCalendarEvents(0, 10)).rejects.toThrow(
        expectedError.message
      )
    })
  })

  describe('upsertCalendarEvent', async () => {
    it('should upsert a calendar event', async () => {
      const mockEvent: CalendarEvent = {
        id: 'someId',
        title: 'someTitle',
        description: 'someDescription',
        startDate: new Date(2025, 9, 21, 12, 0, 0, 0),
        endDate: new Date(2025, 9, 21, 13, 0, 0, 0),
      }
      global.fetch = vi.fn(() =>
        Promise.resolve({
          ok: true,
          json: () => Promise.resolve(mockEvent),
        })
      ) as Mock

      const event = await client.upsertCalendarEvent(mockEvent)
      expect(event).toEqual(mockEvent)
      expect(fetch).toHaveBeenCalledWith(`${baseUrl}/api/Events`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(mockEvent),
      })
    })

    it('should throw an error if the response is not ok', async () => {
      const expectedError = new Error('Some error')
      global.fetch = vi.fn(() =>
        Promise.resolve({
          ok: false,
          statusText: expectedError.message,
        })
      ) as Mock

      const mockEvent: CalendarEvent = {
        id: 'someId',
        title: 'someTitle',
        description: 'someDescription',
        startDate: new Date(2025, 9, 21, 12, 0, 0, 0),
        endDate: new Date(2025, 9, 21, 13, 0, 0, 0),
      }
      await expect(client.upsertCalendarEvent(mockEvent)).rejects.toThrow(
        expectedError.message
      )
    })
  })
})
