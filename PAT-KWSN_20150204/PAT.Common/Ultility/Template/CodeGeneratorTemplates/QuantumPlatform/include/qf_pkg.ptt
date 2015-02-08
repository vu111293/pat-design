//////////////////////////////////////////////////////////////////////////////
// Product: QF/C++
// Last Updated for Version: 4.3.00
// Date of the Last Update:  Nov 02, 2011
//
//                    Q u a n t u m     L e a P s
//                    ---------------------------
//                    innovating embedded systems
//
// Copyright (C) 2002-2011 Quantum Leaps, LLC. All rights reserved.
//
// This software may be distributed and modified under the terms of the GNU
// General Public License version 2 (GPL) as published by the Free Software
// Foundation and appearing in the file GPL.TXT included in the packaging of
// this file. Please note that GPL Section 2[b] requires that all works based
// on this software must also be made publicly available under the terms of
// the GPL ("Copyleft").
//
// Alternatively, this software may be distributed and modified under the
// terms of Quantum Leaps commercial licenses, which expressly supersede
// the GPL and are specifically designed for licensees interested in
// retaining the proprietary status of their code.
//
// Contact information:
// Quantum Leaps Web site:  http://www.quantum-leaps.com
// e-mail:                  info@quantum-leaps.com
//////////////////////////////////////////////////////////////////////////////
#ifndef qf_pkg_h
#define qf_pkg_h

/// \file
/// \ingroup qf
/// \brief Internal (package scope) QF/C++ interface.

#include "qf_port.h"                                                // QF port

#ifdef Q_USE_NAMESPACE
namespace QP {
#endif
                                    // QF-specific interrupt locking/unlocking
#ifndef QF_CRIT_STAT_TYPE
    /// \brief This is an internal macro for defining the critical section
    /// status type.
    ///
    /// The purpose of this macro is to enable writing the same code for the
    /// case when critical sectgion status type is defined and when it is not.
    /// If the macro #QF_CRIT_STAT_TYPE is defined, this internal macro
    /// provides the definition of the critical section status varaible.
    /// Otherwise this macro is empty.
    /// \sa #QF_CRIT_STAT_TYPE
    #define QF_CRIT_STAT_

    /// \brief This is an internal macro for entering a critical section.
    ///
    /// The purpose of this macro is to enable writing the same code for the
    /// case when critical sectgion status type is defined and when it is not.
    /// If the macro #QF_CRIT_STAT_TYPE is defined, this internal macro
    /// invokes #QF_CRIT_ENTRY passing the key variable as the parameter.
    /// Otherwise #QF_CRIT_ENTRY is invoked with a dummy parameter.
    /// \sa #QF_CRIT_ENTRY
    #define QF_CRIT_ENTRY_()    QF_CRIT_ENTRY(dummy)

    /// \brief This is an internal macro for exiting a cricial section.
    ///
    /// The purpose of this macro is to enable writing the same code for the
    /// case when critical sectgion status type is defined and when it is not.
    /// If the macro #QF_CRIT_STAT_TYPE is defined, this internal macro
    /// invokes #QF_CRIT_EXIT passing the key variable as the parameter.
    /// Otherwise #QF_CRIT_EXIT is invoked with a dummy parameter.
    /// \sa #QF_CRIT_EXIT
    #define QF_CRIT_EXIT_()     QF_CRIT_EXIT(dummy)

#else
    #define QF_CRIT_STAT_       QF_CRIT_STAT_TYPE critStat_;
    #define QF_CRIT_ENTRY_()    QF_CRIT_ENTRY(critStat_)
    #define QF_CRIT_EXIT_()     QF_CRIT_EXIT(critStat_)
#endif

// package-scope objects -----------------------------------------------------
extern QTimeEvt *QF_timeEvtListHead_;  ///< head of linked list of time events
extern QF_EPOOL_TYPE_ QF_pool_[QF_MAX_EPOOL];        ///< allocate event pools
extern uint8_t QF_maxPool_;                  ///< # of initialized event pools
extern QSubscrList *QF_subscrList_;             ///< the subscriber list array
extern QSignal QF_maxSignal_;                ///< the maximum published signal

//............................................................................
/// \brief Structure representing a free block in the Native QF Memory Pool
/// \sa ::QMPool
struct QFreeBlock {
    QFreeBlock *m_next;
};

/// \brief access to the poolId of an event \a e_
#define EVT_POOL_ID(e_)     ((e_)->poolId_)

/// \brief access to the refCtr of an event \a e_
#define EVT_REF_CTR(e_)     ((e_)->refCtr_)

/// \brief increment the refCtr of an event \a e_
#define EVT_INC_REF_CTR(e_) (++((QEvent *)(e_))->refCtr_)

/// \brief decrement the refCtr of an event \a e_
#define EVT_DEC_REF_CTR(e_) (--((QEvent *)(e_))->refCtr_)

#ifdef Q_USE_NAMESPACE
}                                                              // namespace QP
#endif

#endif                                                             // qf_pkg_h

