import {defineStore} from "pinia";
import {ref} from "vue";
import type {Player} from "@/Models/RoomModels";

interface UserState {
    user: Player | null;
    token: string | null;
}

const USER_STATE_STORAGE_KEY = 'userState';

export const useUserStore = defineStore('user', () => {
    const state = ref<UserState>(loadState());

    function loadState(): UserState {
        console.debug('Loading user state from local storage');
        const savedState = localStorage.getItem(USER_STATE_STORAGE_KEY);
        return savedState ? JSON.parse(savedState) : { user: null, token: null };
    }

    function setUser(user: Player, token: string): void {
        console.debug('Setting user state:', user);
        state.value = { user: user, token: token };
        saveState();
    }

    function logout(): void {
        state.value = { user: null, token: null };
        saveState();
    }

    function saveState(): void {
        console.debug('Saving user state to local storage');
        localStorage.setItem(USER_STATE_STORAGE_KEY, JSON.stringify(state.value));
    }

    function getToken(): string | null {
        console.debug('Getting token from user state');
        const savedState = localStorage.getItem(USER_STATE_STORAGE_KEY);
        return savedState ? JSON.parse(savedState).token : null;
    }

    return {
        get user() { console.debug("test");return state.value.user },
        getToken,
        setUser,
        logout
    }
});
